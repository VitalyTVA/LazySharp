﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

#if ROSLYN_NEW
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Syntax = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation;
using CompilationOptions = Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions;
using SyntaxTree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree;
using SyntaxTreeBase = Microsoft.CodeAnalysis.SyntaxTree;
#else
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.CSharp;
using Roslyn.Compilers.Common;
using SyntaxTreeBase = Roslyn.Compilers.CSharp.SyntaxTree;
#endif

namespace LazySharp.Roslyn {
    class Program {
        static string path = @"..\..\";
        static void Main(string[] args) {
            if(Directory.GetCurrentDirectory().EndsWith("Roslyn") || Directory.GetCurrentDirectory().EndsWith("Roslyn_new"))
                path += @"..\";

            const string prototypesDllName = "LazySharp.Prototypes.dll";
            const string generatedDllName = "LazySharp.Generated.dll";

            var listGenericTree = GetTree(@"Prototypes\List.Generic");
            var listTree = GetTree(@"Prototypes\List");

            Compilation prototypesCompilation = Compilation.Create(prototypesDllName)
                                        .AddReferences(
                                            new MetadataFileReference(typeof(object).Assembly.Location),
                                            new MetadataFileReference(typeof(Enumerable).Assembly.Location)
                                         ).WithOptions(new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                                        .AddSyntaxTrees(
                                            GetTree(@"Utils\Argument"),
                                            GetTree(@"L"),
                                            listGenericTree,
                                            listTree
                                        );
            EmitAndLog(prototypesCompilation, prototypesDllName);

            var listGenericTreeModifed = GenerateFile(listGenericTree, @"Generated\List.Generic");
            var listTreeModifed = GenerateFile(listTree, @"Generated\List");

            Compilation generatedCompilation = prototypesCompilation
                .AddSyntaxTrees(listGenericTreeModifed)
                .AddSyntaxTrees(listTreeModifed)
                .WithAssemblyName(generatedDllName);
            EmitAndLog(generatedCompilation, generatedDllName);
        }
        static bool EmitAndLog(Compilation compilation, string dllName) {
            var result = compilation.Emit(dllName);
            if(result.Success)
                Console.WriteLine("Emit success: " + dllName);
            else {
                Console.WriteLine("EMIT ERROR: " + dllName);
                
                foreach(var item in result.Diagnostics) {
                    Console.WriteLine(item.ToString());
                }
                
                throw new InvalidOperationException(dllName + " library not emited");
            }
            return result.Success;
        }
        static SyntaxTreeBase GenerateFile(SyntaxTreeBase orginal, string fileName) {
            string newFileName = GetFilePath(fileName);
            string newText = new LazyRewriter().Visit(orginal.GetRoot()).GetText().ToString();
            string oldText = File.Exists(newFileName) ? File.ReadAllText(newFileName) : null;
            if(newText != oldText)
                File.WriteAllText(newFileName, newText);
            return SyntaxTree.ParseFile(newFileName);
        }
        static SyntaxTreeBase GetTree(string fileName) {
            return SyntaxTree.ParseFile(GetFilePath(fileName));
        }
        static string GetFilePath(string fileName) {
            return Path.Combine(path, fileName + ".cs");
        }
    }
}

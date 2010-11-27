/*
Copyright 2010 Google Inc

Licensed under the Apache License, Version 2.0 (the ""License"");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an ""AS IS"" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

using NUnit.Framework;
using log4net;

using Google.Apis.Discovery;


namespace Google.Apis.Tools.CodeGen.Tests
{
	[TestFixture()]
	public class CodeGenTest
	{
		[Test()]
		public void TestCompilationWithDefaultDecorators ()
		{
			var serviceName = "buzz";
			var version = "v1";
			var clientNamespace ="Google.Apis.Samples.CommandLineGeneratedService.Buzz";
			var language = "CSharp";
			
			string cacheDirectory = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
			    "GoogleApis.Tools.CodeGenCache");
			
			if(Directory.Exists(cacheDirectory) == false){
				Directory.CreateDirectory(cacheDirectory);
			}
			
			var webfetcher = new CachedWebDiscoveryDevice(
			    new Uri("http://www.googleapis.com/discovery/0.1/describe?api=" + serviceName),
			    new DirectoryInfo(cacheDirectory));
			var discovery = new DiscoveryService(webfetcher);
			// Build the service based on discovery information.
			var service = discovery.GetService(version);
			
			var generator = new CodeGen(service, clientNamespace, true);			
			var provider = CodeDomProvider.CreateProvider(language);
			
			CompilerParameters cp = new CompilerParameters();

		    // Add an assembly reference.
		    cp.ReferencedAssemblies.Add( "System.dll" );
			AddRefereenceToDelararingAssembly(typeof(DiscoveryService), cp);
			AddRefereenceToDelararingAssembly(typeof(ILog), cp);
			
		    cp.GenerateExecutable = false;		
		    cp.GenerateInMemory = true;
			
			CodeCompileUnit codeCompileUnit = generator.GenerateCode ();
			CompilerResults compilerResults = provider.CompileAssemblyFromDom (cp, codeCompileUnit);
			if ( compilerResults.Errors.Count > 0 )
			{
				var sb = new StringBuilder("Failed To compile resultant code with default decorators. ");
				foreach(var error in compilerResults.Errors)
				{
					sb.AppendLine(error.ToString());
				}
				Assert.Fail(sb.ToString());
			}
		}
		
		private void AddRefereenceToDelararingAssembly(Type target, CompilerParameters cp)
		{
			cp.ReferencedAssemblies.Add(target.Assembly.CodeBase);
		}
	}
}


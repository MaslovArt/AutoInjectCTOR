AutoInjectCTOR 
=========

## Overview
**AutoInjectCTOR** is a simple source generator lib that creates DI constructor.

Source generators do not allow to rewrite users source code, so the target class must be **partial**.

There are two ways for DI constructor generation: mark class or fields with InjectCtorAttribute.
 
 Marked class ctor contains of all readonly non static fields that do't marked with [IgnoreCtorAttribute].
```csharp
[InjectCtor]
public partial class Worker { ... }
```
 Not marked class ctor contains of fields with InjectCtorAttribute.
```csharp
[InjectCtor]
readonly ServiceA serviceA;
```
Source generation result is a partial class ({userClass}..InjectCtor.cs) that contains constructor with all needed parameters.
 ## Configuration
Generator project
```xml
<PropertyGroup>
	<TargetFramework>netstandard2.0</TargetFramework>
	<LangVersion>8</LangVersion>
</PropertyGroup>
 
<ItemGroup>
	<PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="3.8.0" PrivateAssets="all" />
	<PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.2" PrivateAssets="all" />
</ItemGroup>

<PropertyGroup>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild> <!-- Generates a package at build -->
    <IncludeBuildOutput>false</IncludeBuildOutput> <!-- Do not include the generator as a lib dependency -->
</PropertyGroup>

<ItemGroup>
    <!-- Package the generator in the analyzer directory of the nuget package -->
    <None Include="$(OutputPath)\$(AssemblyName).dll" Pack="true" PackagePath="analyzers/dotnet/cs" Visible="false" />
</ItemGroup>
```
User project
```xml
<PropertyGroup>
	<OutputType>Exe</OutputType>
	<TargetFramework>net5.0</TargetFramework>
	<LangVersion>8</LangVersion>
</PropertyGroup>
 
<PropertyGroup>
	<!--Create files for generated code-->
	<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
	<CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)\GeneratedFiles</CompilerGeneratedFilesOutputPath>
</PropertyGroup>

<ItemGroup>
	<!--Reference to generator project-->
	<ProjectReference Include="..\AutoInjectCTOR\AutoInjectCTOR.csproj">
		<OutputItemType>Analyzer</OutputItemType>
		<ReferenceOutputAssembly>True</ReferenceOutputAssembly>
	</ProjectReference>
</ItemGroup>
```


## Example
```csharp
[InjectCtor]
public partial class Worker
{
    readonly ServiceA serviceA;
    readonly ServiceB serviceB;
    readonly ServiceC serviceC;
    [IgnoreCtor]
    readonly ServiceC serviceD;
...
```
or
```csharp
public partial class Worker
{
	[InjectCtor]
    readonly ServiceA serviceA;
    [InjectCtor]
    readonly ServiceB serviceB;
    [InjectCtor]
    readonly ServiceC serviceC;
    readonly ServiceC serviceD;
...
```
Source generator result (Worker.InjectCtor.cs):
```csharp
public partial class Worker 
{
	public Worker(
		AutoInjectExample.Services.ServiceA serviceA,
		AutoInjectExample.Services.ServiceB serviceB,
		AutoInjectExample.Services.ServiceC serviceC)
	{
		this.serviceA = serviceA;
		this.serviceB = serviceB;
		this.serviceC = serviceC;
	}
}
```
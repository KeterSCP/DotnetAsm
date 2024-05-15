// TODO: remove
// using Microsoft.CodeAnalysis;
//
// using MonacoRoslynCompletionProvider;
// using MonacoRoslynCompletionProvider.Api;
//
// namespace DotnetAsm.Api.RoslynMonaco;
//
// public static class RoslynMonacoRequestHandler
// {
//     public static async Task<TabCompletionResult[]> Handle(TabCompletionRequest tabCompletionRequest)
//     {
//         var workspace = CompletionWorkspace.Create(tabCompletionRequest.Assemblies);
//         var document = await workspace.CreateDocument(tabCompletionRequest.Code, OutputKind.ConsoleApplication);
//         return await document.GetTabCompletion(tabCompletionRequest.Position, CancellationToken.None);
//     }
//
//     public static async Task<HoverInfoResult> Handle(HoverInfoRequest hoverInfoRequest)
//     {
//         var workspace = CompletionWorkspace.Create(hoverInfoRequest.Assemblies);
//         var document = await workspace.CreateDocument(hoverInfoRequest.Code, OutputKind.ConsoleApplication);
//         return await document.GetHoverInformation(hoverInfoRequest.Position, CancellationToken.None);
//     }
//
//     public static async Task<CodeCheckResult[]> Handle(CodeCheckRequest codeCheckRequest)
//     {
//         var workspace = CompletionWorkspace.Create(codeCheckRequest.Assemblies);
//         var document = await workspace.CreateDocument(codeCheckRequest.Code, OutputKind.ConsoleApplication);
//         return await document.GetCodeCheckResults(CancellationToken.None);
//     }
//
//     public static async Task<SignatureHelpResult> Handle(SignatureHelpRequest signatureHelpRequest)
//     {
//         var workspace = CompletionWorkspace.Create(signatureHelpRequest.Assemblies);
//         var document = await workspace.CreateDocument(signatureHelpRequest.Code, OutputKind.ConsoleApplication);
//         return await document.GetSignatureHelp(signatureHelpRequest.Position, CancellationToken.None);
//     }
// }

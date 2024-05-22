import * as monaco from "monaco-editor";

export const themeName = "dotnet-asm-theme";
export const themeNameDark = "dotnet-asm-theme-dark";

// Cannot have different themes: https://github.com/microsoft/monaco-editor/issues/338
monaco.editor.defineTheme(themeName, {
  base: "vs",
  inherit: true,
  rules: [
    // C#
    {
      token: "method",
      foreground: "#DCDCAA",
    },
    {
      token: "enum",
      foreground: "#86C691",
    },
    {
      token: "struct",
      foreground: "#86C691",
    },
    // ASM x86
    {
      token: "comment-xarch-asm",
      foreground: "#11AA22",
    },
    {
      token: "keyword-xarch-asm",
      foreground: "#FF6600",
    },
    {
      token: "register-xarch-asm",
      foreground: "#FFCC00",
      fontStyle: "italic",
    },
    {
      token: "directive-xarch-asm",
      foreground: "#FFCC00",
      fontStyle: "italic",
    },
    {
      token: "GM-label-xarch-asm",
      foreground: "#FFCC00",
    },
    {
      token: "offset-xarch-asm",
      foreground: "#339999",
    },
    {
      token: "number-xarch-asm",
      foreground: "#339999",
    },
    {
      token: "exception-xarch-asm",
      fontStyle: "bold italic",
      foreground: "#FF0000",
    },
  ],
  colors: {},
});

monaco.editor.defineTheme(themeNameDark, {
  base: "vs-dark",
  inherit: true,
  rules: [
    // C#
    {
      token: "method",
      foreground: "#DCDCAA",
    },
    {
      token: "enum",
      foreground: "#86C691",
    },
    {
      token: "struct",
      foreground: "#86C691",
    },
    // ASM x86
    {
      token: "comment-xarch-asm",
      foreground: "#11AA22",
    },
    {
      token: "keyword-xarch-asm",
      foreground: "#FF6600",
    },
    {
      token: "register-xarch-asm",
      foreground: "#FFCC00",
      fontStyle: "italic",
    },
    {
      token: "directive-xarch-asm",
      foreground: "#FFCC00",
      fontStyle: "italic",
    },
    {
      token: "GM-label-xarch-asm",
      foreground: "#FFCC00",
    },
    {
      token: "offset-xarch-asm",
      foreground: "#339999",
    },
    {
      token: "number-xarch-asm",
      foreground: "#339999",
    },
    {
      token: "exception-xarch-asm",
      fontStyle: "bold italic",
      foreground: "#FF0000",
    },
  ],
  colors: {},
});

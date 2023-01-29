import { ref } from "vue";

const themeDarkStorageName = "theme-dark";

const darkTheme = ref((localStorage.getItem(themeDarkStorageName) ?? "true") === "true");
export default darkTheme;

export function saveTheme(dark: boolean) {
    localStorage.setItem(themeDarkStorageName, dark.toString());
}

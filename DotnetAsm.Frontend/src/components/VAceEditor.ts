import ace from "ace-builds";
import "ace-builds/src-noconflict/ext-language_tools";
import { defineComponent, markRaw, h } from "vue";

export const VAceEditor = defineComponent({
    data() {
        return {
            contentBackup: "",
            isSettingContent: false,
            editor: {} as ace.Ace.Editor,
        };
    },
    props: {
        value: {
            type: String,
            required: true,
        },
        lang: {
            type: String,
            default: "text",
        },
        theme: {
            type: String,
            default: "chrome",
        },
        options: Object,
        placeholder: String,
        readonly: Boolean,
        wrap: Boolean,
        printMargin: {
            type: [Boolean, Number],
            default: true,
        },
        minLines: Number,
        maxLines: Number,
    },
    emits: ["update:value", "init"],
    render() {
        return h("div");
    },
    mounted() {
        ace.require("ace/ext/language_tools");

        this.contentBackup = this.value;

        this.editor = markRaw(
            ace.edit(this.$el, {
                placeholder: this.placeholder,
                readOnly: this.readonly,
                value: this.value,
                mode: "ace/mode/" + this.lang,
                theme: "ace/theme/" + this.theme,
                wrap: this.wrap,
                printMargin: this.printMargin,
                useWorker: false,
                minLines: this.minLines,
                maxLines: this.maxLines,
                ...this.options,
            })
        );

        this.editor.on("change", () => {
            if (this.isSettingContent) return;

            const content = this.editor.getValue();
            this.contentBackup = content;

            this.$emit("update:value", content);
        });

        this.$emit("init", this.editor);
    },
    watch: {
        value(val) {
            if (this.contentBackup !== val) {
                try {
                    this.isSettingContent = true;
                    this.editor.setValue(val, 1);
                } finally {
                    this.isSettingContent = false;
                }
                this.contentBackup = val;
            }
        },
        theme(val) {
            this.editor.setTheme("ace/theme/" + val);
        },
        options(val) {
            this.editor.setOptions(val);
        },
        readonly(val) {
            this.editor.setReadOnly(val);
        },
        placeholder(val) {
            this.editor.setOption("placeholder", val);
        },
        wrap(val) {
            this.editor.setWrapBehavioursEnabled(val);
        },
        printMargin(val) {
            this.editor.setOption("printMargin", val);
        },
        lang(val) {
            this.editor.setOption("mode", "ace/mode/" + val);
        },
        minLines(val) {
            this.editor.setOption("minLines", val);
        },
        maxLines(val) {
            this.editor.setOption("maxLines", val);
        },
    },
});

import * as monaco from "monaco-editor";

export const languageName = "ryujit-xarch-asm";
export const themeName = `${languageName}-theme`;
export const themeNameDark = `${languageName}-theme-dark`;

monaco.languages.register({
    id: languageName,
});

// Note, that it is not valid x86 assembly, it may contain pseudoinstructions and keywords like gword
monaco.languages.setMonarchTokensProvider(languageName, {
    ignoreCase: true,
    tokenizer: {
        root: [
            [
                /\b(?:aaa|aad|aam|aas|adc|add|addpd|addps|addsd|addss|addsubpd|addsubps|aesdec|aesdeclast|aesenc|aesenclast|aesimc|aeskeygenassist|and|andpd|andps|andnpd|andnps|arpl|blendpd|blendps|blendvpd|blendvps|bound|bsf|bsr|bswap|bt|btc|btr|bts|cbw|cwde|cdqe|clc|cld|cflush|clts|cmc|cmov(?:n?e|ge?|ae?|le?|be?|n?o|n?z)|cmp|cmppd|cmpps|cmps|cnpsb|cmpsw|cmpsd|cmpsq|cmpss|cmpxchg|cmpxchg8b|cmpxchg16b|comisd|comiss|cpuid|crc32|cvtdq2pd|cvtdq2ps|cvtpd2dq|cvtpd2pi|cvtpd2ps|cvtpi2pd|cvtpi2ps|cvtps2dq|cvtps2pd|cvtps2pi|cvtsd2si|cvtsd2ss|cvts2sd|cvtsi2ss|cvtss2sd|cvtss2si|cvttpd2dq|cvtpd2pi|cvttps2dq|cvttps2pi|cvttps2dq|cvttps2pi|cvttsd2si|cvttss2si|cwd|cdq|cqo|daa|das|dec|div|divpd|divps|divsd|divss|dppd|dpps|emms|enter|extractps|f2xm1|fabs|fadd|faddp|fiadd|fbld|fbstp|fchs|fclex|fnclex|fcmov(?:n?e|ge?|ae?|le?|be?|n?o|n?z)|fcom|fcmop|fcompp|fcomi|fcomip|fucomi|fucomip|fcos|fdecstp|fdiv|fdivp|fidiv|fdivr|fdivrp|fidivr|ffree|ficom|ficomp|fild|fincstp|finit|fnint|fist|fistp|fisttp|fld|fld1|fldl2t|fldl2e|fldpi|fldlg2|fldln2|fldz|fldcw|fldenv|fmul|fmulp|fimul|fnop|fpatan|fprem|fprem1|fptan|frndint|frstor|fsave|fnsave|fscale|fsin|fsincos|fsqrt|fst|fstp|fstcw|fnstcw|fstenv|fnstenv|fsts|fnstsw|fsub|fsubp|fisub|fsubr|fsubrp|fisubr|ftst|fucom|fucomp|fucompp|fxam|fxch|fxrstor|fxsave|fxtract|fyl2x|fyl2xp1|haddpd|haddps|husbpd|hsubps|idiv|imul|in|inc|ins|insb|insw|insd|insertps|int|into|invd|invplg|invpcid|iret|iretd|iretq|lahf|lar|lddqu|ldmxcsr|lds|les|lfs|lgs|lss|lea|leave|lfence|lgdt|lidt|llgdt|lmsw|lock|lods|lodsb|lodsw|lodsd|lodsq|lsl|ltr|maskmovdqu|maskmovq|maxpd|maxps|maxsd|maxss|mfence|minpd|minps|minsd|minss|monitor|mov|movapd|movaps|movbe|movd|movq|movddup|movdqa|movdqu|movq2q|movhlps|movhpd|movhps|movlhps|movlpd|movlps|movmskpd|movmskps|movntdqa|movntdq|movnti|movntpd|movntps|movntq|movq|movq2dq|movs|movsb|movsw|movsd|movsq|movsd|movshdup|movsldup|movss|movsx|movsxd|movupd|movups|movzx|mpsadbw|mul|mulpd|mulps|mulsd|mulss|mwait|neg|not|or|orpd|orps|out|outs|outsb|outsw|outsd|pabsb|pabsw|pabsd|packsswb|packssdw|packusdw|packuswbpaddb|paddw|paddd|paddq|paddsb|paddsw|paddusb|paddusw|palignr|pand|pandn|pause|pavgb|pavgw|pblendvb|pblendw|pclmulqdq|pcmpeqb|pcmpeqw|pcmpeqd|pcmpeqq|pcmpestri|pcmpestrm|pcmptb|pcmptgw|pcmpgtd|pcmpgtq|pcmpistri|pcmpisrm|pextrb|pextrd|pextrq|pextrw|phaddw|phaddd|phaddsw|phinposuw|phsubw|phsubd|phsubsw|pinsrb|pinsrd|pinsrq|pinsrw|pmaddubsw|pmadddwd|pmaxsb|pmaxsd|pmaxsw|pmaxsw|pmaxub|pmaxud|pmaxuw|pminsb|pminsd|pminsw|pminub|pminud|pminuw|pmovmskb|pmovsx|pmovzx|pmuldq|pmulhrsw|pmulhuw|pmulhw|pmulld|pmullw|pmuludw|pop|popa|popad|popcnt|popf|popfd|popfq|por|prefetch|psadbw|pshufb|pshufd|pshufhw|pshuflw|pshufw|psignb|psignw|psignd|pslldq|psllw|pslld|psllq|psraw|psrad|psrldq|psrlw|psrld|psrlq|psubb|psubw|psubd|psubq|psubsb|psubsw|psubusb|psubusw|test|ptest|punpckhbw|punpckhwd|punpckhdq|punpckhddq|punpcklbw|punpcklwd|punpckldq|punpckldqd|push|pusha|pushad|pushf|pushfd|pxor|prcl|rcr|rol|ror|rcpps|rcpss|rdfsbase|rdgsbase|rdmsr|rdpmc|rdrand|rdtsc|rdtscp|rep|repe|repz|repne|repnz|roundpd|roundps|roundsd|roundss|rsm|rsqrps|rsqrtss|sahf|sal|sar|shl|shr|sbb|scas|scasb|scasw|scasd|set(?:n?e|ge?|ae?|le?|be?|n?o|n?z)|sfence|sgdt|shld|shrd|shufpd|shufps|sidt|sldt|smsw|sqrtpd|sqrtps|sqrtsd|sqrtss|stc|std|stmxcsr|stos|stosb|stosw|stosd|stosq|str|sub|subpd|subps|subsd|subss|swapgs|syscall|sysenter|sysexit|sysret|teset|ucomisd|ucomiss|ud2|unpckhpd|unpckhps|unpcklpd|unpcklps|vaddsd|vaddss|vbroadcast|vcvtph2ps|vcvtp2sph|verr|verw|vextractf128|vinsertf128|vmaskmov|vmovdqa|vmovsd|vmovss|vpermilpd|vpermilps|vperm2f128|vtestpd|vtestps|vxorps|vzeroall|vzeroupper|wait|fwait|wbinvd|wrfsbase|wrgsbase|wrmsr|xadd|xchg|xgetbv|xlat|xlatb|xor|xorpd|xorps|xrstor|xsave|xsaveopt|xsetbv|lzcnt|extrq|insertq|movntsd|movntss|vfmaddpd|vfmaddps|vfmaddsd|vfmaddss|vfmaddsubbpd|vfmaddsubps|vfmsubaddpd|vfmsubaddps|vfmsubpd|vfmsubps|vfmsubsd|vfnmaddpd|vfnmaddps|vfnmaddsd|vfnmaddss|vfnmsubpd|vfnmusbps|vfnmusbsd|vfnmusbss|cvt|xor|cli|sti|hlt|nop|lock|wait|enter|leave|ret|loop(?:n?e|n?z)?|call|j(?:mp|n?e|ge?|ae?|le?|be?|n?o|n?z))\b/,
                "keyword-xarch-asm",
            ],
            [
                /\b(?:CS|DS|ES|FS|GS|SS|RAX|EAX|RBX|EBX|RCX|ECX|RDX|EDX|RCX|RIP|EIP|IP|RBP|RSP|ESP|SP|RSI|ESI|SI|RDI|EDI|DI|RFLAGS|EFLAGS|FLAGS|R(?:[8-9]|1[0-5])d?|(?:Y|X)MM(?:[0-9]|10|11|12|13|14|15)|(?:A|B|C|D)(?:X|H|L)|CR(?:[0-4]|DR(?:[0-7]|TR6|TR7|EFER)))\b/,
                "register-xarch-asm",
            ],
            [
                /\b(?:d[bwdqtoy]|res[bwdqto]|equ|times|align|alignb|sectalign|section|ptr|byte|word|bword|dword|gword|qword|xmmword|short|incbin)\b/,
                "directive-xarch-asm",
            ],
            [/\bG_M\d+_IG\d+:?\b/, "GM-label-xarch-asm"],
            [/\b[0-9]+H\b/, "offset-xarch-asm"],
            [/\s[0-9]+\b/, "number-xarch-asm"],
            [/;.*/, "comment-xarch-asm"],
            ["int3", "exception-xarch-asm"],
        ],
    },
});

monaco.editor.defineTheme(themeName, {
    base: "vs",
    inherit: true,
    rules: [
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

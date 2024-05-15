import Signatures from "./Signatures";

export default class SignatureHelpResult {
  signatures!: Signatures[];
  activeParameter!: number;
  activeSignature!: number;
}

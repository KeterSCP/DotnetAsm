import TargetFramework from "./TargetFramework";

export default class AsmGenerationRequest {
  csharpCode!: string;
  methodName?: string;
  generateSummary!: boolean;
  usePgo!: boolean;
  useTieredCompilation!: boolean;
  useReadyToRun!: boolean;
  targetFramework!: TargetFramework;

  constructor(
    csharpCode: string,
    methodName: string,
    generateSummary: boolean,
    usePgo: boolean,
    useTieredCompilation: boolean,
    useReadyToRun: boolean,
    targetFramework: TargetFramework
  ) {
    this.csharpCode = csharpCode;
    this.methodName = methodName;
    this.generateSummary = generateSummary;
    this.usePgo = usePgo;
    this.useTieredCompilation = useTieredCompilation;
    this.useReadyToRun = useReadyToRun;
    this.targetFramework = targetFramework;
  }
}

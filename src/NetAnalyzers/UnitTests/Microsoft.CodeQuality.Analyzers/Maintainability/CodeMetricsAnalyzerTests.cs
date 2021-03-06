// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Test.Utilities;
using Xunit;
using VerifyCS = Test.Utilities.CSharpCodeFixVerifier<
    Microsoft.CodeQuality.Analyzers.Maintainability.CodeMetrics.CodeMetricsAnalyzer,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;
using VerifyVB = Test.Utilities.VisualBasicCodeFixVerifier<
    Microsoft.CodeQuality.Analyzers.Maintainability.CodeMetrics.CodeMetricsAnalyzer,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace Microsoft.CodeQuality.Analyzers.Maintainability.CodeMetrics.UnitTests
{
    public class CodeMetricsAnalyzerTests
    {
        #region CA1501: Avoid excessive inheritance

        [Fact]
        public async Task CA1501_CSharp_VerifyDiagnostic()
        {
            var source = @"
class BaseClass { }
class FirstDerivedClass : BaseClass { }
class SecondDerivedClass : FirstDerivedClass { }
class ThirdDerivedClass : SecondDerivedClass { }
class FourthDerivedClass : ThirdDerivedClass { }

// This class violates the rule.
class FifthDerivedClass : FourthDerivedClass { }
";
            DiagnosticResult[] expected = new[] {
                // Test0.cs(9, 7): warning CA1501: 'FifthDerivedClass' has an object hierarchy '6' levels deep within the defining module. If possible, eliminate base classes within the hierarchy to decrease its hierarchy level below '6': 'FourthDerivedClass, ThirdDerivedClass, SecondDerivedClass, FirstDerivedClass, BaseClass, Object'
                GetCSharpCA1501ExpectedDiagnostic(9, 7, "FifthDerivedClass", 6, 6, "FourthDerivedClass, ThirdDerivedClass, SecondDerivedClass, FirstDerivedClass, BaseClass, Object")};
            await VerifyCS.VerifyAnalyzerAsync(source, expected);
        }

        [Fact]
        public async Task CA1501_Basic_VerifyDiagnostic()
        {
            var source = @"
Class BaseClass
End Class

Class FirstDerivedClass
    Inherits BaseClass
End Class

Class SecondDerivedClass
    Inherits FirstDerivedClass
End Class

Class ThirdDerivedClass
    Inherits SecondDerivedClass
End Class

Class FourthDerivedClass
    Inherits ThirdDerivedClass
End Class

Class FifthDerivedClass
    Inherits FourthDerivedClass
End Class
";
            DiagnosticResult[] expected = new[] {
                // Test0.vb(21, 7): warning CA1501: 'FifthDerivedClass' has an object hierarchy '6' levels deep within the defining module. If possible, eliminate base classes within the hierarchy to decrease its hierarchy level below '6': 'FourthDerivedClass, ThirdDerivedClass, SecondDerivedClass, FirstDerivedClass, BaseClass, Object'
                GetBasicCA1501ExpectedDiagnostic(21, 7, "FifthDerivedClass", 6, 6, "FourthDerivedClass, ThirdDerivedClass, SecondDerivedClass, FirstDerivedClass, BaseClass, Object")};
            await VerifyVB.VerifyAnalyzerAsync(source, expected);
        }

        [Fact]
        public async Task CA1501_Configuration_CSharp_VerifyDiagnostic()
        {
            var source = @"
class BaseClass { }
class FirstDerivedClass : BaseClass { }
";
            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1501: 1
";
            DiagnosticResult[] expected = new[] {
                // Test0.cs(3, 7): warning CA1501: 'BaseClass' has an object hierarchy '2' levels deep within the defining module. If possible, eliminate base classes within the hierarchy to decrease its hierarchy level below '2': 'BaseClass, Object'
                GetCSharpCA1501ExpectedDiagnostic(3, 7, "FirstDerivedClass", 2, 2, "BaseClass, Object")};
            await VerifyCSharpAsync(source, additionalText, expected);
        }

        [Fact]
        public async Task CA1501_Configuration_Basic_VerifyDiagnostic()
        {
            var source = @"
Class BaseClass
End Class

Class FirstDerivedClass
    Inherits BaseClass
End Class
";
            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1501: 1
";
            DiagnosticResult[] expected = new[] {
                // Test0.vb(5, 7): warning CA1501: 'BaseClass' has an object hierarchy '2' levels deep within the defining module. If possible, eliminate base classes within the hierarchy to decrease its hierarchy level below '2': 'BaseClass, Object'
                GetBasicCA1501ExpectedDiagnostic(5, 7, "FirstDerivedClass", 2, 2, "BaseClass, Object")};
            await VerifyBasicAsync(source, additionalText, expected);
        }

        #endregion

        #region CA1502: Avoid excessive complexity

        [Fact]
        public async Task CA1502_CSharp_VerifyDiagnostic()
        {
            var source = @"
class C
{
    void M(bool b)
    {
        // Default threshold = 25
        var x = b && b && b && b && b && b && b &&
            b && b && b && b && b && b && b &&
            b && b && b && b && b && b && b &&
            b && b && b && b && b && b && b;
    }
}
";
            DiagnosticResult[] expected = new[] {
                // Test0.cs(4,10): warning CA1502: 'M' has a cyclomatic complexity of '28'. Rewrite or refactor the code to decrease its complexity below '26'.
                GetCSharpCA1502ExpectedDiagnostic(4, 10, "M", 28, 26)};
            await VerifyCS.VerifyAnalyzerAsync(source, expected);
        }

        [Fact]
        public async Task CA1502_Basic_VerifyDiagnostic()
        {
            var source = @"
Class C
    Private Sub M(ByVal b As Boolean)
        Dim x = b AndAlso b AndAlso b AndAlso b AndAlso b AndAlso b AndAlso
            b AndAlso b AndAlso b AndAlso b AndAlso b AndAlso b AndAlso b AndAlso
            b AndAlso b AndAlso b AndAlso b AndAlso b AndAlso b AndAlso b AndAlso
            b AndAlso b AndAlso b AndAlso b AndAlso b AndAlso b AndAlso b AndAlso b
    End Sub
End Class
";
            DiagnosticResult[] expected = new[] {
                // Test0.vb(3,17): warning CA1502: 'M' has a cyclomatic complexity of '28'. Rewrite or refactor the code to decrease its complexity below '26'.
                GetBasicCA1502ExpectedDiagnostic(3, 17, "M", 28, 26)};
            await VerifyVB.VerifyAnalyzerAsync(source, expected);
        }

        [Fact]
        public async Task CA1502_Configuration_CSharp_VerifyDiagnostic()
        {
            var source = @"
class C
{
    void M1(bool b)
    {
        var x = b && b && b && b;
    }

    void M2(bool b)
    {
        var x = b && b;
    }
}
";

            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1502: 2
";
            DiagnosticResult[] expected = new[] {
                // Test0.cs(4,10): warning CA1502: 'M1' has a cyclomatic complexity of '4'. Rewrite or refactor the code to decrease its complexity below '3'.
                GetCSharpCA1502ExpectedDiagnostic(4, 10, "M1", 4, 3)};
            await VerifyCSharpAsync(source, additionalText, expected);
        }

        [Fact]
        public async Task CA1502_Configuration_Basic_VerifyDiagnostic()
        {
            var source = @"
Class C
    Private Sub M1(ByVal b As Boolean)
        Dim x = b AndAlso b AndAlso b AndAlso b
    End Sub

    Private Sub M2(ByVal b As Boolean)
        Dim x = b AndAlso b
    End Sub
End Class
";
            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1502: 2
";
            DiagnosticResult[] expected = new[] {
                // Test0.vb(3,17): warning CA1502: 'M1' has a cyclomatic complexity of '4'. Rewrite or refactor the code to decrease its complexity below '3'.
                GetBasicCA1502ExpectedDiagnostic(3, 17, "M1", 4, 3)};
            await VerifyBasicAsync(source, additionalText, expected);
        }

        [Fact]
        public async Task CA1502_SymbolBasedConfiguration_CSharp_VerifyDiagnostic()
        {
            var source = @"
class C
{
    void M1(bool b)
    {
        var x = b && b && b && b;
    }

    void M2(bool b)
    {
        var x = b && b;
    }
}
";

            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1502(Type): 4
CA1502(Method): 2
";
            DiagnosticResult[] expected = new[] {
                // Test0.cs(2,7): warning CA1502: 'C' has a cyclomatic complexity of '6'. Rewrite or refactor the code to decrease its complexity below '5'.
                GetCSharpCA1502ExpectedDiagnostic(2, 7, "C", 6, 5),
                // Test0.cs(4,10): warning CA1502: 'M1' has a cyclomatic complexity of '4'. Rewrite or refactor the code to decrease its complexity below '3'.
                GetCSharpCA1502ExpectedDiagnostic(4, 10, "M1", 4, 3)};
            await VerifyCSharpAsync(source, additionalText, expected);
        }

        [Fact]
        public async Task CA1502_SymbolBasedConfiguration_Basic_VerifyDiagnostic()
        {
            var source = @"
Class C
    Private Sub M1(ByVal b As Boolean)
        Dim x = b AndAlso b AndAlso b AndAlso b
    End Sub

    Private Sub M2(ByVal b As Boolean)
        Dim x = b AndAlso b
    End Sub
End Class
";
            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1502(Type): 4
CA1502(Method): 2
";
            DiagnosticResult[] expected = new[] {
                // Test0.vb(2,7): warning CA1502: 'C' has a cyclomatic complexity of '6'. Rewrite or refactor the code to decrease its complexity below '5'.
                GetBasicCA1502ExpectedDiagnostic(2, 7, "C", 6, 5),
                // Test0.vb(3,17): warning CA1502: 'M1' has a cyclomatic complexity of '4'. Rewrite or refactor the code to decrease its complexity below '3'.
                GetBasicCA1502ExpectedDiagnostic(3, 17, "M1", 4, 3)};
            await VerifyBasicAsync(source, additionalText, expected);
        }

        #endregion

        #region CA1505: Avoid unmaintainable code

        [Fact]
        public async Task CA1505_Configuration_CSharp_VerifyDiagnostic()
        {
            var source = @"
class C
{
    void M1(bool b)
    {
        var x = b && b && b && b;
    }
}
";

            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1505: 95
";
            DiagnosticResult[] expected = new[] {
                // Test0.cs(2,7): warning CA1505: 'C' has a maintainability index of '91'. Rewrite or refactor the code to increase its maintainability index (MI) above '94'.
                GetCSharpCA1505ExpectedDiagnostic(2, 7, "C", 91, 94),
                // Test0.cs(4,10): warning CA1505: 'M1' has a maintainability index of '91'. Rewrite or refactor the code to increase its maintainability index (MI) above '94'.
                GetCSharpCA1505ExpectedDiagnostic(4, 10, "M1", 91, 94)};
            await VerifyCSharpAsync(source, additionalText, expected);
        }

        [Fact]
        public async Task CA1505_Configuration_Basic_VerifyDiagnostic()
        {
            var source = @"
Class C
    Private Sub M1(ByVal b As Boolean)
        Dim x = b AndAlso b AndAlso b AndAlso b
    End Sub
End Class
";

            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1505: 95
";
            DiagnosticResult[] expected = new[] {
                // Test0.vb(2,7): warning CA1505: 'C' has a maintainability index of '91'. Rewrite or refactor the code to increase its maintainability index (MI) above '94'.
                GetBasicCA1505ExpectedDiagnostic(2, 7, "C", 91, 94),
                // Test0.vb(3,17): warning CA1505: 'M1' has a maintainability index of '91'. Rewrite or refactor the code to increase its maintainability index (MI) above '94'.
                GetBasicCA1505ExpectedDiagnostic(3, 17, "M1", 91, 94)};
            await VerifyBasicAsync(source, additionalText, expected);
        }

        [Fact]
        public async Task CA1505_SymbolBasedConfiguration_CSharp_VerifyDiagnostic()
        {
            var source = @"
class C
{
    void M1(bool b)
    {
        var x = b && b && b && b;
    }
}
";

            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1505(Type): 95
";
            DiagnosticResult[] expected = new[] {
                // Test0.cs(2,7): warning CA1505: 'C' has a maintainability index of '91'. Rewrite or refactor the code to increase its maintainability index (MI) above '94'.
                GetCSharpCA1505ExpectedDiagnostic(2, 7, "C", 91, 94)};
            await VerifyCSharpAsync(source, additionalText, expected);
        }

        [Fact]
        public async Task CA1505_SymbolBasedConfiguration_Basic_VerifyDiagnostic()
        {
            var source = @"
Class C
    Private Sub M1(ByVal b As Boolean)
        Dim x = b AndAlso b AndAlso b AndAlso b
    End Sub
End Class
";

            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1505(Type): 95
";
            DiagnosticResult[] expected = new[] {
                // Test0.vb(2,7): warning CA1505: 'C' has a maintainability index of '91'. Rewrite or refactor the code to increase its maintainability index (MI) above '94'.
                GetBasicCA1505ExpectedDiagnostic(2, 7, "C", 91, 94)};
            await VerifyBasicAsync(source, additionalText, expected);
        }

        #endregion

        #region CA1506: Avoid excessive class coupling

        [Fact]
        public async Task CA1506_Configuration_CSharp_VerifyDiagnostic()
        {
            var source = @"
class C
{
    void M1(C1 c1, C2 c2, C3 c3, N.C4 c4)
    {
    }
}

class C1 { }
class C2 { }
class C3 { }
namespace N { class C4 { } }
";
            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1506: 2
";
            DiagnosticResult[] expected = new[] {
                // Test0.cs(2,7): warning CA1506: 'C' is coupled with '4' different types from '2' different namespaces. Rewrite or refactor the code to decrease its class coupling below '3'.
                GetCSharpCA1506ExpectedDiagnostic(2, 7, "C", 4, 2, 3),
                // Test0.cs(4,10): warning CA1506: 'M1' is coupled with '4' different types from '2' different namespaces. Rewrite or refactor the code to decrease its class coupling below '3'.
                GetCSharpCA1506ExpectedDiagnostic(4, 10, "M1", 4, 2, 3)};
            await VerifyCSharpAsync(source, additionalText, expected);
        }

        [Fact, WorkItem(2133, "https://github.com/dotnet/roslyn-analyzers/issues/2133")]
        public async Task CA1506_Configuration_CSharp_Linq()
        {
            var source = @"
using System.Linq;
using System.Collections.Generic;
class C
{
    IEnumerable<int> TestCa1506()
    {
        var ints = new[] { 1, 2 };
        return from a in ints
               from b in ints
               from c in ints
               from d in ints
               from e in ints
               from f in ints
               from g in ints 
               from h in ints
               from i in ints
               from j in ints
               from k in ints
               from l in ints
               from m in ints
               from n in ints
               from o in ints
               from p in ints
               select p;
    }
}
";
            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1506: 2
";
            DiagnosticResult[] expected = new[] {
                // Test0.cs(4,7): warning CA1506: 'C' is coupled with '4' different types from '3' different namespaces. Rewrite or refactor the code to decrease its class coupling below '3'.
                GetCSharpCA1506ExpectedDiagnostic(4, 7, "C", 4, 3, 3),
                // Test0.cs(4,10): warning CA1506: 'TestCa1506' is coupled with '4' different types from '3' different namespaces. Rewrite or refactor the code to decrease its class coupling below '3'.
                GetCSharpCA1506ExpectedDiagnostic(6, 22, "TestCa1506", 4, 3, 3)};
            await VerifyCSharpAsync(source, additionalText, expected);
        }

        [Fact]
        public async Task CA1506_Configuration_Basic_VerifyDiagnostic()
        {
            var source = @"
Class C
    Private Sub M1(c1 As C1, c2 As C2, c3 As C3, c4 As N.C4)
    End Sub
End Class

Class C1
End Class

Class C2
End Class

Class C3
End Class

Namespace N
    Class C4
    End Class
End Namespace
";
            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1506: 2
";
            DiagnosticResult[] expected = new[] {
                // Test0.vb(2,7): warning CA1506: 'C' is coupled with '4' different types from '2' different namespaces. Rewrite or refactor the code to decrease its class coupling below '3'.
                GetBasicCA1506ExpectedDiagnostic(2, 7, "C", 4, 2, 3),
                // Test0.vb(3,17): warning CA1506: 'M1' is coupled with '4' different types from '2' different namespaces. Rewrite or refactor the code to decrease its class coupling below '3'.
                GetBasicCA1506ExpectedDiagnostic(3, 17, "M1", 4, 2, 3)};
            await VerifyBasicAsync(source, additionalText, expected);
        }

        [Fact]
        public async Task CA1506_SymbolBasedConfiguration_CSharp_VerifyDiagnostic()
        {
            var source = @"
class C
{
    void M1(C1 c1, C2 c2, C3 c3, N.C4 c4)
    {
    }
}

class C1 { }
class C2 { }
class C3 { }
namespace N { class C4 { } }
";
            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1506(Method): 2
CA1506(Type): 10
";
            DiagnosticResult[] expected = new[] {
                // Test0.cs(4,10): warning CA1506: 'M1' is coupled with '4' different types from '2' different namespaces. Rewrite or refactor the code to decrease its class coupling below '3'.
                GetCSharpCA1506ExpectedDiagnostic(4, 10, "M1", 4, 2, 3)};
            await VerifyCSharpAsync(source, additionalText, expected);
        }

        [Fact]
        public async Task CA1506_SymbolBasedConfiguration_Basic_VerifyDiagnostic()
        {
            var source = @"
Class C
    Private Sub M1(c1 As C1, c2 As C2, c3 As C3, c4 As N.C4)
    End Sub
End Class

Class C1
End Class

Class C2
End Class

Class C3
End Class

Namespace N
    Class C4
    End Class
End Namespace
";
            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA1506(Method): 2
CA1506(Type): 10
";
            DiagnosticResult[] expected = new[] {
                // Test0.vb(3,17): warning CA1506: 'M1' is coupled with '4' different types from '2' different namespaces. Rewrite or refactor the code to decrease its class coupling below '3'.
                GetBasicCA1506ExpectedDiagnostic(3, 17, "M1", 4, 2, 3)};
            await VerifyBasicAsync(source, additionalText, expected);
        }

        #endregion

        #region CA1509: Invalid entry in code metrics rule specification file

        [Fact]
        public async Task CA1509_VerifyDiagnostics()
        {
            var source = @"";

            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

# 1. Multiple colons
CA1501: 1 : 2

# 2. Whitespace in RuleId
CA 1501: 1

# 3. Invalid Code Metrics RuleId
CA1600: 1

# 4. Non-integral Threshold.
CA1501: None

# 5. Not supported SymbolKind.
CA1501(Local): 1

# 6. Missing SymbolKind.
CA1501(: 1

# 7. Missing CloseParens after SymbolKind.
CA1501(Method: 1

# 8. Multiple SymbolKinds.
CA1501(Method)(Type): 1

# 9. Missing Threshold.
CA1501
";
            DiagnosticResult[] expected = new[] {
                // CodeMetricsConfig.txt(6,1): warning CA1509: Invalid entry 'CA1501: 1 : 2' in code metrics rule specification file 'CodeMetricsConfig.txt'
                GetCA1509ExpectedDiagnostic(6, 1, "CA1501: 1 : 2", AdditionalFileName),
                // CodeMetricsConfig.txt(9,1): warning CA1509: Invalid entry 'CA 1501: 1' in code metrics rule specification file 'CodeMetricsConfig.txt'
                GetCA1509ExpectedDiagnostic(9, 1, "CA 1501: 1", AdditionalFileName),
                // CodeMetricsConfig.txt(12,1): warning CA1509: Invalid entry 'CA1600: 1' in code metrics rule specification file 'CodeMetricsConfig.txt'
                GetCA1509ExpectedDiagnostic(12, 1, "CA1600: 1", AdditionalFileName),
                // CodeMetricsConfig.txt(15,1): warning CA1509: Invalid entry 'CA1501: None' in code metrics rule specification file 'CodeMetricsConfig.txt'
                GetCA1509ExpectedDiagnostic(15, 1, "CA1501: None", AdditionalFileName),
                // CodeMetricsConfig.txt(18,1): warning CA1509: Invalid entry 'CA1501(Local): 1' in code metrics rule specification file 'CodeMetricsConfig.txt'
                GetCA1509ExpectedDiagnostic(18, 1, "CA1501(Local): 1", AdditionalFileName),
                // CodeMetricsConfig.txt(21,1): warning CA1509: Invalid entry 'CA1501(: 1' in code metrics rule specification file 'CodeMetricsConfig.txt'
                GetCA1509ExpectedDiagnostic(21, 1, "CA1501(: 1", AdditionalFileName),
                // CodeMetricsConfig.txt(24,1): warning CA1509: Invalid entry 'CA1501(Method: 1' in code metrics rule specification file 'CodeMetricsConfig.txt'
                GetCA1509ExpectedDiagnostic(24, 1, "CA1501(Method: 1", AdditionalFileName),
                // CodeMetricsConfig.txt(27,1): warning CA1509: Invalid entry 'CA1501(Method)(Type): 1' in code metrics rule specification file 'CodeMetricsConfig.txt'
                GetCA1509ExpectedDiagnostic(27, 1, "CA1501(Method)(Type): 1", AdditionalFileName),
                // CodeMetricsConfig.txt(30,1): warning CA1509: Invalid entry 'CA1501' in code metrics rule specification file 'CodeMetricsConfig.txt'
                GetCA1509ExpectedDiagnostic(30, 1, "CA1501", AdditionalFileName)};
            await VerifyCSharpAsync(source, additionalText, expected);
        }

        [Fact]
        public async Task CA1509_NoDiagnostics()
        {
            var source = @"";

            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

# 1. Duplicates are allowed
CA1501: 1
CA1501: 2

# 2. Duplicate RuleId-SymbolKind pairs are allowed.
CA1501(Method): 1
CA1501(Method): 1

# 3. All valid symbol kinds
CA1502(Assembly): 1
CA1502(Namespace): 1
CA1502(Type): 1
CA1502(Method): 1
CA1502(Field): 1
CA1502(Property): 1
CA1502(Event): 1

# 4. Whitespaces before and after the key-value pair are allowed.
   CA1501: 1        

# 5. Whitespaces before and after the colon are allowed.
CA1501    :    1
";
            await VerifyCSharpAsync(source, additionalText);
        }

        [Fact]
        public async Task CA1509_VerifyNoMetricDiagnostics()
        {
            // Ensure we don't report any code metric diagnostics when we have invalid entries in code metrics configuration file.
            var source = @"
class BaseClass { }
class FirstDerivedClass : BaseClass { }
class SecondDerivedClass : FirstDerivedClass { }
class ThirdDerivedClass : SecondDerivedClass { }
class FourthDerivedClass : ThirdDerivedClass { }

// This class violates the CA1501 rule for default threshold.
class FifthDerivedClass : FourthDerivedClass { }";

            string additionalText = @"
# FORMAT:
# 'RuleId'(Optional 'SymbolKind'): 'Threshold'

CA 1501: 10
";
            DiagnosticResult[] expected = new[] {
                // CodeMetricsConfig.txt(5,1): warning CA1509: Invalid entry 'CA 1501: 10' in code metrics rule specification file 'CodeMetricsConfig.txt'
                GetCA1509ExpectedDiagnostic(5, 1, "CA 1501: 10", AdditionalFileName)};
            await VerifyCSharpAsync(source, additionalText, expected);
        }

        #endregion

        #region Helpers
        private static DiagnosticResult GetCSharpCA1501ExpectedDiagnostic(int line, int column, string symbolName, int metricValue, int threshold, string baseTypes)
            => VerifyCS.Diagnostic(CodeMetricsAnalyzer.CA1501Rule)
                .WithLocation(line, column)
                .WithArguments(symbolName, metricValue, threshold, baseTypes);

        private static DiagnosticResult GetBasicCA1501ExpectedDiagnostic(int line, int column, string symbolName, int metricValue, int threshold, string baseTypes)
            => VerifyVB.Diagnostic(CodeMetricsAnalyzer.CA1501Rule)
                .WithLocation(line, column)
                .WithArguments(symbolName, metricValue, threshold, baseTypes);

        private static DiagnosticResult GetCSharpCA1502ExpectedDiagnostic(int line, int column, string symbolName, int metricValue, int threshold)
            => VerifyCS.Diagnostic(CodeMetricsAnalyzer.CA1502Rule)
                .WithLocation(line, column)
                .WithArguments(symbolName, metricValue, threshold);

        private static DiagnosticResult GetBasicCA1502ExpectedDiagnostic(int line, int column, string symbolName, int metricValue, int threshold)
            => VerifyVB.Diagnostic(CodeMetricsAnalyzer.CA1502Rule)
                .WithLocation(line, column)
                .WithArguments(symbolName, metricValue, threshold);

        private static DiagnosticResult GetCSharpCA1505ExpectedDiagnostic(int line, int column, string symbolName, int metricValue, int threshold)
            => VerifyCS.Diagnostic(CodeMetricsAnalyzer.CA1505Rule)
                .WithLocation(line, column)
                .WithArguments(symbolName, metricValue, threshold);

        private static DiagnosticResult GetBasicCA1505ExpectedDiagnostic(int line, int column, string symbolName, int metricValue, int threshold)
            => VerifyVB.Diagnostic(CodeMetricsAnalyzer.CA1505Rule)
                .WithLocation(line, column)
                .WithArguments(symbolName, metricValue, threshold);

        private static DiagnosticResult GetCSharpCA1506ExpectedDiagnostic(int line, int column, string symbolName, int coupledTypesCount, int namespaceCount, int threshold)
            => VerifyCS.Diagnostic(CodeMetricsAnalyzer.CA1506Rule)
                .WithLocation(line, column)
                .WithArguments(symbolName, coupledTypesCount, namespaceCount, threshold);

        private static DiagnosticResult GetBasicCA1506ExpectedDiagnostic(int line, int column, string symbolName, int coupledTypesCount, int namespaceCount, int threshold)
            => VerifyVB.Diagnostic(CodeMetricsAnalyzer.CA1506Rule)
                .WithLocation(line, column)
                .WithArguments(symbolName, coupledTypesCount, namespaceCount, threshold);

        private static DiagnosticResult GetCA1509ExpectedDiagnostic(int line, int column, string entry, string additionalFile)
            => VerifyCS.Diagnostic(CodeMetricsAnalyzer.InvalidEntryInCodeMetricsConfigFileRule)
                .WithLocation(additionalFile, line, column)
                .WithArguments(entry, additionalFile);

        private const string AdditionalFileName = "CodeMetricsConfig.txt";

        private async Task VerifyCSharpAsync(string source, string codeMetricsConfigSource, params DiagnosticResult[] expected)
        {
            var csharpTest = new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { source },
                    AdditionalFiles = { (AdditionalFileName, codeMetricsConfigSource) }
                }
            };

            csharpTest.ExpectedDiagnostics.AddRange(expected);

            await csharpTest.RunAsync();
        }

        private async Task VerifyBasicAsync(string source, string codeMetricsConfigSource, params DiagnosticResult[] expected)
        {
            var vbTest = new VerifyVB.Test
            {
                TestState =
                {
                    Sources = { source },
                    AdditionalFiles = { (AdditionalFileName, codeMetricsConfigSource) }
                }
            };

            vbTest.ExpectedDiagnostics.AddRange(expected);

            await vbTest.RunAsync();
        }

        #endregion
    }
}

/*  Copyright (c) 2021 Damian Brunold, Gesundheitsdirektion Kanton Zürich

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class TestSdepEinzelfallpruefungen
    {
        [Test]
        public void Test2016VersicherungsklasseVorhanden()
        {
            TestCaseInterpreter
                .Test("if _1_4_V02 is in [1, 2, 3, 4] then C14 is in [1, 2, 3]")
                .With("_1_4_V02", 1)
                .With("C14", 1)
                .ExpectTrue()
                .With("_1_4_V02", 5)
                .With("C14", 1)
                .ExpectTrue()
                .With("_1_4_V02", 1)
                .With("C14", 4)
                .ExpectFalse()
                .With("_1_4_V02", 1)
                .With("C14", null)
                .ExpectFalse()
                .With("_1_4_V02", null)
                .With("C14", 1)
                .ExpectTrue()
                .Execute();
        }

        [Test]
        public void Test2018EintrittsartNeugeborene()
        {
            TestCaseInterpreter
                .Test("if _2_1_V01 == 'MN' then _1_2_V03 == 3")
                .With("_2_1_V01", "MN")
                .With("_1_2_V03", 3)
                .ExpectTrue()
                .With("_2_1_V01", "MN")
                .With("_1_2_V03", 2)
                .ExpectFalse()
                .With("_2_1_V01", "MX")
                .With("_1_2_V03", 2)
                .ExpectTrue()
                .Execute();
        }
        
        [Test]
        public void Test2020AhvNummerVorhanden()
        {
            TestCaseInterpreter
                .Test("if is_null(_1_1_V04) then return TRUE; " +
                      "if (_1_1_V04 is numerical exact_len 4) or (_1_1_V04 == 'CHE') " +
                      "then not is_null(C8) and C8 matches //^[0-9]{3}[.][0-9]{4}[.][0-9]{4}[.][0-9]{2}$//")
                .With("_1_1_V04", null)
                .With("C8", 3)
                .ExpectTrue()
                .With("_1_1_V04", "8820")
                .With("C8", null)
                .ExpectFalse()
                .With("_1_1_V04", "8820")
                .With("C8", "123.1234.1234.12")
                .ExpectTrue()
                .With("_1_1_V04", "CHE")
                .With("C8", "123.1234.1234.12")
                .ExpectTrue()
                .With("_1_1_V04", "8820")
                .With("C8", "1234.123.1234.12")
                .ExpectFalse()
                .Execute();
        }
        
        [Test]
        public void Test2021IpsKostenUndStunden()
        {
            TestCaseInterpreter
                .Test("if B37 > 0 then _1_3_V03 > 0")
                .With("B37", null)
                .With("_1_3_V03", 3)
                .ExpectTrue()
                .With("B37", 0)
                .With("_1_3_V03", 3)
                .ExpectTrue()
                .With("B37", 1)
                .With("_1_3_V03", 3)
                .ExpectTrue()
                .With("B37", 1)
                .With("_1_3_V03", 0)
                .ExpectFalse()
                .Execute();
        }
        
        
        [Test]
        public void Test2022GrundversicherungNeugeborene()
        {
            TestCaseInterpreter
                .Test("if _1_1_V03 == 0 and (C13 == '300' or C13 == '320') and ((_1_1_V04 is numerical exact_len 4) or (_1_1_V04 == 'CHE')) and _1_1_V05 == 'CHE' " +
                      "then _1_4_V02 is in [1, 2, 3, 4]")
                .With("_1_1_V03", 0)
                .With("_1_1_V04", "8820")
                .With("_1_1_V05", "CHE")
                .With("C13", "300")
                .With("_1_4_V02", 1)
                .ExpectTrue()
                .With("_1_1_V03", 0)
                .With("_1_1_V04", "CHE")
                .With("_1_1_V05", "CHE")
                .With("C13", "300")
                .With("_1_4_V02", 1)
                .ExpectTrue()
                .With("_1_1_V03", 0)
                .With("_1_1_V04", "8820")
                .With("_1_1_V05", "CHE")
                .With("C13", "320")
                .With("_1_4_V02", 1)
                .ExpectTrue()
                .With("_1_1_V03", 0)
                .With("_1_1_V04", "abcd")
                .With("_1_1_V05", "FRA")
                .With("C13", "300")
                .With("_1_4_V02", 1)
                .ExpectTrue()
                .With("_1_1_V03", 1)
                .With("_1_1_V04", "8820")
                .With("_1_1_V05", "CHE")
                .With("C13", "300")
                .With("_1_4_V02", 1)
                .ExpectTrue()
                .With("_1_1_V03", 0)
                .With("_1_1_V04", "8820")
                .With("_1_1_V05", "CHE")
                .With("C13", "300")
                .With("_1_4_V02", 5)
                .ExpectFalse()
                .Execute();
        }
        
        [Test]
        public void Test2023KrankenversichererKorrekt()
        {
            TestCaseInterpreter
                .Test("if _1_4_V02 == 1 then (C11 in KRANKENVERSICHERUNG or C11 == 9999)")
                .WithList("KRANKENVERSICHERUNG", 1234, 1235, 1236)
                .With("_1_4_V02", 1)
                .With("C11", 1234)
                .ExpectTrue()
                .WithList("KRANKENVERSICHERUNG", 1234, 1235, 1236)
                .With("_1_4_V02", 1)
                .With("C11", 9999)
                .ExpectTrue()
                .WithList("KRANKENVERSICHERUNG", 1234, 1235, 1236)
                .With("_1_4_V02", 1)
                .With("C11", 1200)
                .ExpectFalse()
                .WithList("KRANKENVERSICHERUNG", 1234, 1235, 1236)
                .With("_1_4_V02", 0)
                .With("C11", 9999)
                .ExpectTrue()
                .Execute();
        }
        
        [Test]
        public void Test2027HkstZhVorhanden()
        {
            TestCaseInterpreter
                .Test("C13 in HKST_SDEP")
                .WithList("HKST_SDEP", "100", "110", "120")
                .With("C13", "100")
                .ExpectTrue()
                .WithList("HKST_SDEP", "100", "110", "120")
                .With("C13", "130")
                .ExpectFalse()
                .WithList("HKST_SDEP", "100", "110", "120")
                .With("C13", null)
                .ExpectFalse()
                .Execute();
        }
        
        [Test]
        public void Test2031ZivilrechtlicherWohnkantonVorhanden()
        {
            TestCaseInterpreter
                .Test("if is_null(_1_1_V04) then return TRUE; " +
                      "if (_1_1_V04 is numerical exact_len 4) or (_1_1_V04 == 'CHE') " +
                      "then (C12 in KANTON)")
                .WithList("KANTON", "ZH", "BE", "AG")
                .With("_1_1_V04", null)
                .With("C12", "ZH")
                .ExpectTrue()
                .WithList("KANTON", "ZH", "BE", "AG")
                .With("_1_1_V04", "8820")
                .With("C12", "ZH")
                .ExpectTrue()
                .WithList("KANTON", "ZH", "BE", "AG")
                .With("_1_1_V04", "8820")
                .With("C12", "BS")
                .ExpectFalse()
                .WithList("KANTON", "ZH", "BE", "AG")
                .With("_1_1_V04", "CHE")
                .With("C12", "ZH")
                .ExpectTrue()
                .WithList("KANTON", "ZH", "BE", "AG")
                .With("_1_1_V04", "8820")
                .With("C12", "")
                .ExpectFalse()
                .WithList("KANTON", "ZH", "BE", "AG")
                .With("_1_1_V04", "8820")
                .With("C12", null)
                .ExpectFalse()
                .Execute();
        }

        [Test]
        public void Test2037BURNummerZOFehltOderFalsch()
        {
            TestCaseInterpreter
                .Test("C22 is alphanumerical exact_len 8")
                .With("C22", "12345678")
                .ExpectTrue()
                .With("C22", "abc45678")
                .ExpectTrue()
                .With("C22", "123456789")
                .ExpectFalse()
                .With("C22", "1234567")
                .ExpectFalse()
                .Execute();
        }
        
        [Test]
        public void Test2038FallnummerZOFehlt()
        {
            TestCaseInterpreter
                .Test("C23 is not empty")
                .With("C23", "")
                .ExpectFalse()
                .With("C23", "1234")
                .ExpectTrue()
                .With("C23", null)
                .ExpectFalse()
                .With("C23", 1234)
                .ExpectTrue()
                .Execute();
        }
        
        [Test]
        public void Test2039GlnNummerZONichtKorrekt()
        {
            TestCaseInterpreter
                .Test("C24 is numerical exact_len 13")
                .With("C24", "")
                .ExpectFalse()
                .With("C24", "1234")
                .ExpectFalse()
                .With("C24", null)
                .ExpectFalse()
                .With("C24", "1234567890123")
                .ExpectTrue()
                .With("C24", "123456789012a")
                .ExpectFalse()
                .Execute();
        }
        
        [Test]
        public void Test2040UngueltigeAngabeBeiZORolle()
        {
            TestCaseInterpreter
                .Test("C25 is in [1, 2]")
                .With("C25", 1)
                .ExpectTrue()
                .With("C25", 2)
                .ExpectTrue()
                .With("C25", 3)
                .ExpectFalse()
                .Execute();
        }

        [Test]
        public void Test2435ZORecordFehltOhneAnreicherung()
        {
            TestCaseInterpreter
                .Test("if is_null(MFZO_ZO_Missing) then return TRUE;\r\n" +
                      "not str_contains(MFZO_ZO_Missing, 'URO1.1.7,')")
                .With("MFZO_ZO_Missing", null)
                .ExpectTrue()
                .Execute();
        }
        

        [Test]
        public void Test2435ZORecordFehltOhneEintrag()
        {
            TestCaseInterpreter
                .Test("if is_null(MFZO_ZO_Missing) then return TRUE;\r\n" +
                      "not str_contains(MFZO_ZO_Missing, 'URO1.1.7,')")
                .With("MFZO_ZO_Missing", "GYNT,BEW7.2.1,")
                .ExpectTrue()
                .Execute();
        }

        [Test]
        public void Test2435ZORecordFehltMitEintrag()
        {
            TestCaseInterpreter
                .Test("if is_null(MFZO_ZO_Missing) then return TRUE;\r\n" +
                      "not str_contains(MFZO_ZO_Missing, 'URO1.1.7,')")
                .With("MFZO_ZO_Missing", "GYNT,URO1.1.7,BEW7.2.1,")
                .ExpectFalse()
                .Execute();
        }
        
        [Test]
        public void TestTemplate()
        {
            TestCaseInterpreter
                .Test("TRUE")
                .With("_1_1_V04", null)
                .With("C8", 3)
                .ExpectTrue()
                .Execute();
        }
    }
    
}

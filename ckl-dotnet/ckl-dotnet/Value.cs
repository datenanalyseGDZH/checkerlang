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
namespace CheckerLang
{
    public abstract class Value
    {
        public string info = "";
        
        public abstract bool IsEquals(Value value);

        public abstract int CompareTo(Value value);

        public abstract string Type();

        public abstract int HashCode();

        public virtual ValueString AsString()
        {
            throw new ControlErrorException("Cannot convert to string");
        }

        public virtual ValueInt AsInt()
        {
            throw new ControlErrorException("Cannot convert to int");
        }

        public virtual ValueDecimal AsDecimal()
        {
            throw new ControlErrorException("Cannot convert to decimal");
        }

        public virtual ValueBoolean AsBoolean()
        {
            throw new ControlErrorException("Cannot convert to boolean");
        }

        public virtual ValuePattern AsPattern()
        {
            throw new ControlErrorException("Cannot convert to pattern");
        }

        public virtual ValueDate AsDate()
        {
            throw new ControlErrorException("Cannot convert to date");
        }

        public virtual ValueList AsList()
        {
            throw new ControlErrorException("Cannot convert to list");
        }

        public virtual ValueSet AsSet()
        {
            throw new ControlErrorException("Cannot convert to set");
        }

        public virtual ValueMap AsMap()
        {
            throw new ControlErrorException("Cannot convert to map");
        }

        public virtual ValueFunc AsFunc()
        {
            throw new ControlErrorException("Cannot convert to func");
        }

        public virtual ValueError AsError()
        {
            throw new ControlErrorException("Cannot convert to error");
        }

        public virtual ValueInput AsInput()
        {
            throw new ControlErrorException("Cannot convert to input");
        }

        public virtual ValueOutput AsOutput()
        {
            throw new ControlErrorException("Cannot convert to output");
        }

        public virtual ValueNode AsNode()
        {
            throw new ControlErrorException("Cannot convert to node");
        }
        
        public virtual ValueNull AsNull()
        {
            throw new ControlErrorException("Cannot convert to NULL");
        }

        public virtual ValueControlReturn AsReturn()
        {
            throw new ControlErrorException("Cannot convert to return");
        }

        public virtual ValueControlBreak AsBreak()
        {
            throw new ControlErrorException("Cannot convert to break");
        }

        public virtual ValueControlContinue AsContinue()
        {
            throw new ControlErrorException("Cannot convert to continue");
        }

        public virtual bool IsString()
        {
            return false;
        }

        public virtual bool IsInt()
        {
            return false;
        }

        public virtual bool IsDecimal()
        {
            return false;
        }

        public virtual bool IsBoolean()
        {
            return false;
        }

        public virtual bool IsDate()
        {
            return false;
        }

        public virtual bool IsPattern()
        {
            return false;
        }

        public virtual bool IsList()
        {
            return false;
        }

        public virtual bool IsSet()
        {
            return false;
        }

        public virtual bool IsMap()
        {
            return false;
        }

        public virtual bool IsFunc()
        {
            return false;
        }

        public virtual bool IsError()
        {
            return false;
        }

        public virtual bool IsInput()
        {
            return false;
        }

        public virtual bool IsOutput()
        {
            return false;
        }

        public virtual bool IsNode()
        {
            return false;
        }
        
        public virtual bool IsNull()
        {
            return false;
        }

        public virtual bool IsReturn()
        {
            return false;
        }

        public virtual bool IsBreak()
        {
            return false;
        }

        public virtual bool IsContinue()
        {
            return false;
        }

        public bool IsCollection()
        {
            return IsList() || IsSet();
        }
        
        public bool IsAtomic()
        {
            return IsString() || IsInt() || IsDecimal() || IsBoolean() || IsDate() || IsPattern() || IsNull();
        }

        public bool IsNumerical()
        {
            return IsInt() || IsDecimal();
        }

        public Value WithInfo(string info)
        {
            this.info = info;
            return this;
        }

    }
}

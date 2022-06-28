using System.Numerics;
using System;
using System.Text;

class BaseNum{
    private int baseN;
    private char charMax;
    private string number;
    private long valueDec;
    private bool bigger10;

    public long getVal()
    {
        return valueDec;
    }
    public string getNumber()
    {
        return number;
    }
    public char getCharMax()
    {
       return charMax;
    }

    public int getBase()
    {
        return baseN;
    }

    public BaseNum(int baseN)
    {
        this.baseN = baseN;
        if (baseN <= 10)
        {
            bigger10 = false;
            this.charMax=(char)('0'+baseN-1);
        }
        else
        {
            bigger10 = true;
            this.charMax=(char)('A'+baseN-11);
        }
    }

    private bool validateInput(string input)
    {
        if (bigger10 == false)
            for (int i = 0; i < input.Length; i++)
            {
                //Validate: input matches \d+ 
                if (input[i] < '0' || input[i] > charMax) return false;
            }
        else
            for (int i = 0; i < input.Length; i++)
            {
                //Validate: input match (0-9A-<charMax>)+
                if (!(input[i] >= '0' && input[i] <= '9' || input[i]>='A' && input[i]<=charMax)) return false;
            }
        return true;
    }

    public bool readNumber()
    {
        if (!bigger10) Console.Write("Enter number (valid char: [0-{0}]): ", charMax);
        else Console.Write("Enter number (valid char: [0-9A-{0}a-{1}]: ", charMax, (char)(charMax+'a'-'A'));
        string input = Console.ReadLine();
        input = input.ToUpper();
        if (!validateInput(input)) 
        { 
            Console.WriteLine("Input contains 1 or more invalid character(s)"); 
            return false; 
        }
        number = input;
        if (baseN==10) valueDec=long.Parse(number);
        else ConvertToDec();
        return true;
    }

    private void ConvertToDec()
    {
        long val = 0;
        long coef = 1;
        //value = sum(num[i]*base^i)
        for (int i = number.Length-1; i>=0; i--)
        {
            if (number[i] >= '0' && number[i] <= '9')
            {
                val += (number[i] - '0') * coef;
            }
            else
            {
                val += (number[i] - 'A' + 10) * coef;
            }
            coef *= baseN; //coef=base^i
        }
        valueDec = val;
    }
    
    public void setValue(long Value)
    {
        valueDec = Value;
    }

    public void ConvertToBase()
    {
        StringBuilder num=new StringBuilder();
        long val = valueDec;  
        while (val != 0)
        {
            char digit = (char)(val%baseN + (val%baseN>9?'A'-10:'0'));
            num.Insert(0,digit);
            val = val/baseN;
        }
        number = num.ToString();
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("************** Test the program! **************");
        Console.Write("Enter the number's Base (2 <= base <= 36): ");
        var input = Console.ReadLine();
        if (!int.TryParse(input, out int n))
        {
            Console.WriteLine("Error: Input is not a number!");
            return;
        }
        if (n > 36)
        {
            Console.WriteLine("Error: Base's bigger than 36!");
            return;
        }
        if (n <= 1)
        {
            Console.WriteLine("Error: Base's smaller than 2!");
            return;
        }
        BaseNum num1 = new BaseNum(n);

        
        if (!num1.readNumber())
        {
            return;
        }

        Console.Write("Convert to which base (2 <= base <= 36)?  ");
        var to = Console.ReadLine();
        if (!int.TryParse(to, out int toBase))
        {
            Console.WriteLine("Error: Input is not a number!");
            return;
        }
        if (toBase > 36)
        {
            Console.WriteLine("Error: Base's bigger than 36!");
            return;
        }
        if (toBase <= 1)
        {
            Console.WriteLine("Error: Base's smaller than 2!");
            return;
        }

        if(toBase == 10)
        {
            Console.WriteLine("Number after converted to base 10 = {0}", num1.getVal());
            return;
        }

        BaseNum num2 = new BaseNum(toBase);
        num2.setValue(num1.getVal());
        num2.ConvertToBase();
        Console.WriteLine("Number after converted to base {0} = {1}", toBase, num2.getNumber());
    }
}

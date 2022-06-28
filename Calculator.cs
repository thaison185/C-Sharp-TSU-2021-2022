using System.Text.RegularExpressions;


Console.WriteLine("********** CALCULATOR **********");
Console.WriteLine("For more information, use command: help");
Console.WriteLine();

string input="";
bool verbose = false;
while (input!="exit")
{
    Console.Write(">_ ");
    input = Console.ReadLine();
    switch (input)
    {
        case "help":
            Console.WriteLine("   Accepted character: 0123456789()_+-*/^");
            Console.WriteLine("   For the divide a/b, we compute a div b");
            Console.WriteLine("   Verbose mode is disable by default. To enable verbose mode, use command: verbose enable");
            Console.WriteLine("   To disable verbose mode, use command: verbose disable");
            Console.WriteLine("   To clear screen, use command: clrscr");
            Console.WriteLine("   To close program, use command: exit");
            Console.WriteLine();
            break;

        case "verbose enable":
            verbose = true;
            Console.WriteLine("   Verbose mode is enabled!");
            Console.WriteLine();
            break;

        case "verbose disable":
            verbose=false;
            Console.WriteLine("   Verbose mode is disabled!");
            Console.WriteLine();
            break;

        case "clrscr":
            Console.Clear();
            Console.WriteLine("****************** CALCULATOR ******************");
            Console.WriteLine("For more information, use command: help");
            Console.WriteLine();
            break;

        case "exit":
            break;

        default:
            RPN expression = new RPN(input);
            if (!expression.Err())
            {
                try
                {
                    expression.Calculate(verbose);
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine();
                }
            }
            break;
    }
}



//Class Operator - To get some attribute of operator in a Polish Notation
class Operator
{
    private char type;
    private int priority;

    public Operator(char type)
    {
        this.type = type;
        //Priority:
        //  Unary Minus(_): 3
        //  Power(^): 2
        //  mul,div(*,/): 1
        //  add,sub(+,-): 0
        switch (type)
        {
            case '_':
                this.priority = 3;
                break;
            case '^':
                this.priority = 2;
                break;
            case '*':
                this.priority = 1;
                break;
            case '/':
                this.priority = 1;
                break;
            case '(':
                this.priority= -1;
                break;
            default:
                this.priority = 0;
                break;
        }
    }

    public int Priority()
    {
        return priority;
    }

    public char Type()
    {
        return type;
    }
}

//Class: Reverse Polish Notation
class RPN
{
    //Polish notation
    private string input;

    //Reverse Polish Notation
    private Queue <string> reverse = new Queue<string>();

    //Error flag
    private bool err = false;

    public bool Err()
    {
        return err;
    }
    
    //Constructor
    public RPN(string input)
    {
        if (validateInput(input))
        {
            this.input = input;
            try
            {
                ConvertToRPN();
            }
            catch (Exception ex)
            {
                err = true;
                Console.WriteLine(ex.Message);
                Console.WriteLine();
            };
        }
        else
        {
            this.input = "";
            err = true;
            Console.WriteLine();
        }

        
    }

    private bool validateInput(string input)
    {
        //Check if input contains invalid characters. Valid character: numbers and ()+-*/^
        string pattern = @"^[0-9\+\-\*\/\^\(\)_]+$";
        Regex regex = new Regex(pattern);
        if (regex.IsMatch(input))
        {
            return true;
        }
        Console.WriteLine("Err: Input contain invalid character!");
        return false;
    }

    private void ConvertToRPN()
    {
        //Stack  to contain PN elements
        Stack<Operator> elements = new Stack<Operator>();

        //Temp string to contain operands
        string tempStr = "";

        //Flag for error
        bool error=false;

        //Main idea: Read the input left to right.
        //If number(operand), put directly into Queue.
        //If operator, if in the stack is lower priority: Push to stack, else pop to Queue until meet lower priority
        //If open parenthesis: Push to stack
        //If close parenthesis: Pop to Queue until popping open parenthesis

        for (int i = 0; i < input.Length; i++)
        {
            if(Char.IsDigit(input[i])){
                tempStr += input[i];
                if (i == input.Length - 1)
                {
                    reverse.Enqueue(tempStr);
                }
            }
            else
            {
                reverse.Enqueue(tempStr);
                tempStr = "";

                Operator op = new Operator(input[i]);

                switch (input[i])
                {
                    case '(':
                        elements.Push(op);
                        break;
                    
                    case ')':
                        while (elements.Count > 0 && elements.Peek().Type() != '(')
                        {
                            reverse.Enqueue(elements.Pop().Type().ToString());
                        }
                        if (elements.Count == 0)
                        {
                            throw new Exception("Err: Mismatch in input - too many close parentheses!");
                            error = true;
                        }
                        else elements.Pop();
                        break;

                    default:
                        while (elements.Count > 0 && op.Priority() <= elements.Peek().Priority())
                        {
                            reverse.Enqueue(elements.Pop().Type().ToString());
                        }
                        elements.Push(op);
                        break;
                }
            }
            if (error)
            {
                return;
            }
        }

        //After read all input, pop all remain in stack into the queue
        while (elements.Count > 0)
        {
            if (elements.Peek().Type() != '(')
            {
                reverse.Enqueue(elements.Pop().Type().ToString());
            }
            else elements.Pop();
        }
    }

    //Calculate the result
    public void Calculate(bool verbose = false)
    {
        //Stack to contain operands
        Stack<long> operands = new Stack<long>();

        //A long number
        long operand;
        long num1;
        long num2;

        while(reverse.Count > 0)
        {
            string temp = reverse.Dequeue();
            //If not a long number -> operator
            if (!long.TryParse(temp, out operand))
            {
                switch (temp)
                {
                    //If unary minus
                    case "_":
                        if (!operands.TryPop(out num1))
                        {
                            throw new Exception("Err: Mismatch in Expression - Not enough operand for Operator " + temp);
                            return;
                        }
                        num1 = 0 - num1;
                        operands.Push(num1);
                        if(verbose) Console.WriteLine("_ -> {0}",num1);
                        break;
                    
                    //If binary operators
                    case "+":
                        if (!operands.TryPop(out num2))
                        {
                            throw new Exception("Err: Mismatch in Expression - Not enough operand for Operator " + temp);
                            return;
                        }
                        if (!operands.TryPop(out num1))
                        {
                            throw new Exception("Err: Mismatch in Expression - Not enough operand for Operator " + temp);
                            return;
                        }
                        operands.Push(num1+num2);
                        if (verbose) Console.WriteLine("{0}+{1}={2}", num1, num2, num1 + num2);
                        break;

                    case "-":
                        if (!operands.TryPop(out num2))
                        {
                            throw new Exception("Err: Mismatch in Expression - Not enough operand for Operator " + temp);
                            return;
                        }
                        if (!operands.TryPop(out num1))
                        {
                            throw new Exception("Err: Mismatch in Expression - Not enough operand for Operator " + temp);
                            return;
                        }
                        operands.Push(num1 - num2);
                        if (verbose) Console.WriteLine("{0}-{1}={2}", num1, num2, num1 - num2);
                        break;

                    case "*":
                        if (!operands.TryPop(out num2))
                        {
                            throw new Exception("Err: Mismatch in Expression - Not enough operand for Operator " + temp);
                            return;
                        }
                        if (!operands.TryPop(out num1))
                        {
                            throw new Exception("Err: Mismatch in Expression - Not enough operand for Operator " + temp);
                            return;
                        }
                        operands.Push(num1 * num2);
                        if (verbose) Console.WriteLine("{0}*{1}={2}", num1, num2, num1 * num2);
                        break;

                    case "/":
                        if (!operands.TryPop(out num2))
                        {
                            throw new Exception("Err: Mismatch in Expression - Not enough operand for Operator " + temp);
                            return;
                        }
                        if (!operands.TryPop(out num1))
                        {
                            throw new Exception("Err: Mismatch in Expression - Not enough operand for Operator " + temp);
                            return;
                        }
                        
                        if (num2 == 0)
                        {
                            throw new Exception("Err: Can't divide to 0!");
                            return;
                        }
                        operands.Push(num1 / num2);
                        if (verbose) Console.WriteLine("{0}/{1}={2}", num1, num2, num1 / num2);
                        break;

                    case "^":
                        if (!operands.TryPop(out num2))
                        {
                            throw new Exception("Error: Mismatch in Expression - Not enough operand for Operator " + temp);
                            return;
                        }
                        if (!operands.TryPop(out num1))
                        {
                            throw new Exception("Error: Mismatch in Expression - Not enough operand for Operator " + temp);
                            return;
                        }
                        operands.Push((long)Math.Pow(num1, num2));
                        if (verbose) Console.WriteLine("{0}^{1}={2}", num1, num2, Math.Pow(num1, num2));
                        break;
                }
            }
            //If number -> push to stack
            else
            {
                operands.Push(operand);
            }
        }

        //If there is only 1 operand in stack -> result, else -> error
        if (operands.Count == 1)
        {
            Console.WriteLine("Result = {0}", operands.Pop());
        }
        else
        {
            Console.WriteLine("Err: Mismatch in Expression - Not enough operator");
        }
    }
}


public class TowersOfHanoi
{
    public static void Main(String[] args)
    {
        //Three towers A B C. Move all disks from A to B
        char startTower = 'A';
        char endTower = 'B';
        char tempTower = 'C';

        //Prompt the Input messages.
        //Input must not exceed 50.
        Console.Write("Enter the number of Disks(not exceed 50): ");
        var input = Console.ReadLine();
        int totalDisks;

        //validate input
        if (!int.TryParse(input, out totalDisks))
        {
            Console.WriteLine("Err: Input is not a number!");
            return;
        }
        if (totalDisks > 50)
        {
            Console.WriteLine("Err: Number of Disks is too big. The program will take a lot of time to handle!");
            return;
        }

        if (totalDisks < 1)
        {
            Console.WriteLine("Err: Number of Disks has to be 1 or more!");
            return;
        }

        //Call recursive function.
        solveTowers(totalDisks, startTower, endTower, tempTower);
    }

    private static void solveTowers(int n, char startTower, char endTower, char tempTower)
    {
        if (n > 0)
        {
            //Move n-1 disks on the top to temp
            solveTowers(n - 1, startTower, tempTower, endTower);

            //Move the last disk to destination
            Console.WriteLine("Move disk from " + startTower + " to " + endTower);

            //Move n-1 disks back to destination
            solveTowers(n - 1, tempTower, endTower, startTower);

        }
    }

}
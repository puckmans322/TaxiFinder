using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Transactions;
using System;

Random random = new Random();

// инициализация "карты" и кол-ва такси

Console.WriteLine("Введите ширину N: ");
int x = Convert.ToInt32(Console.ReadLine());
Console.WriteLine("Введите длину M: ");
int y = Convert.ToInt32(Console.ReadLine());
string[,] map = new string[x, y];
Console.WriteLine("Введите кол-во такси: ");
int n = Convert.ToInt32(Console.ReadLine());
int searchAmount = 5;

if (n < 0 || n > (x * y))
{
    Console.WriteLine("Ошибка! Кол-во такси больше, чем кол-во клеток!");
    return;
}

// рандомная инициализация "карты" и кол-ва такси
/*
int x = random.Next(4, 12);
int y = random.Next(4, 12);

string[,] map = new string[x, y];
int n = 0;
int searchAmount = 5;
*/

List<string> taxiList = new List<string>();

// полезные функции

bool pointExists(int X, int Y)
{
    if (X > 0 && Y > 0 && X <= x && Y <= y)
    {
        return true;
    }
    else
    {
        return false;
    }
}

bool alreadyExists(string ID)
{
    foreach (string key in map)
    {
        if (key == ID) return true;
    }
    return false;
}

int[] getTaxiPos(string taxiID)
{
    int[] pos = { -1, -1 };
    for (int X = 0; X < x; X++)
    {
        for (int Y = 0; Y < y; Y++)
        {
            if (map[X, Y] == taxiID)
            {
                pos[0] = X + 1;
                pos[1] = Y + 1;
                break;
            }
        }
    }
    return pos;
}

string getTaxiAtPos(int X, int Y)
{
    if (X > 0 && Y > 0 && X <= x && Y <= y)
    {
        return map[X - 1, Y - 1];
    }
    return null;
}

// выводит список точек в квадрате от X,Y с размером R
int[][] getPointsInSquare(int X, int Y, int R)
{
    int i = 0;
    int l = 0;
    int[][] points = new int[(R * 2 + 1) * 4 - 4][];
    // 4 клетки по прямой
    points[i] = new int[] { X + R, Y };
    i++;
    points[i] = new int[] { X, Y + R };
    i++;
    points[i] = new int[] { X - R, Y };
    i++;
    points[i] = new int[] { X, Y - R };
    i++;
    for (int j = 1; j < R * 2; j++)
    {
        if (j % 2 == 1)
        {
            l = (j / 2) + 1;
        }
        else
        {
            l = -(j / 2);
        }
        points[i] = new int[] { X + R, Y + l };
        i++;
        points[i] = new int[] { X - l, Y + R };
        i++;
        points[i] = new int[] { X - R, Y - l };
        i++;
        points[i] = new int[] { X + l, Y - R };
        i++;
    }
    return points;
}

// берет всё точки в квадрате, кроме изначальной. 
int[][] getAllPointsInSquare(int X, int Y, int R)
{
    int k = 0;
    int[][] points = new int[(2 * R + 1) * (2 * R + 1)][];
    for (int i = X - R; i < X + R + 1; i++)
    {
        for (int j = Y - R; j < Y + R + 1; j++)
        {
            points[k] = new int[] { i, j };
            k++;
        }
    }
    return points;
}

// ищет все точки в радиусе R1, которые не входят в радиус R2
int[][] getPointsInCircle(int X, int Y, double R1, double R2)
{
    int k = 0;
    double curDist = 0;
    int[][] points2 = new int[x * y][];
    int[][] points = getAllPointsInSquare(X, Y, Convert.ToInt32(Math.Floor(R1 + 0.00001)));
    foreach (int[] point in points)
    {
        if (point != null && point.Length == 2)
        {
            curDist = getDist(X, Y, point[0], point[1]);
            if (curDist <= R1 && curDist > R2)
            {
                points2[k] = new int[] { point[0], point[1] };
                k++;
            }
        }
    }
    return points2;
}

double getDist(int X, int Y, int X2, int Y2)
{
    double dist = x * y;
    if (X > 0 && Y > 0 && X <= x && Y <= y && X2 > 0 && Y2 > 0 && X2 <= x && Y2 <= y)
    {
        dist = Math.Sqrt((X2 - X) * (X2 - X) + (Y2 - Y) * (Y2 - Y));
    }
    return dist;
}

//следующие 4 функции не используются, созданы на случай перемещения/обмена места/добавления/удаления такси
void moveTaxi(int X, int Y, int X2, int Y2, string newID)
{
    if (X > 0 && Y > 0 && X <= x && Y <= y && X2 > 0 && Y2 > 0 && X2 <= x && Y2 <= y)
    {
        string taxi = getTaxiAtPos(X, Y);
        if (taxi != null && taxi.Length > 0)
        {
            string taxi2 = getTaxiAtPos(X2, Y2);
            if (taxi == null || taxi.Length <= 0)
            {
                int taxiindex = taxiList.IndexOf(taxi);
                if (taxiindex != -1)
                {
                    taxiList[taxiindex] = newID;
                }
                map[X - 1, Y - 1] = "";
                map[X2 - 1, Y2 - 1] = newID;
            }
        }
    }
}

void swapTaxis(int X, int Y, int X2, int Y2, string newID, string newID2)
{
    // на самом деле не меняет их местами, а просто изменяет их ID
    if (X > 0 && Y > 0 && X <= x && Y <= y && X2 > 0 && Y2 > 0 && X2 <= x && Y2 <= y)
    {
        string taxi = getTaxiAtPos(X, Y);
        if (taxi != null && taxi.Length > 0)
        {
            string taxi2 = getTaxiAtPos(X2, Y2);
            if (taxi2 != null && taxi2.Length > 0)
            {
                map[X - 1, Y - 1] = newID;
                map[X2 - 1, Y2 - 1] = newID2;
                int taxiindex = taxiList.IndexOf(taxi);
                if (taxiindex != -1)
                {
                    taxiList[taxiindex] = newID;
                }
                taxiindex = taxiList.IndexOf(taxi2);
                if (taxiindex != -1)
                {
                    taxiList[taxiindex] = newID2;
                }
            }
        }
    }
}

void addTaxi(int X, int Y, string newID)
{
    if (X > 0 && Y > 0 && X <= x && Y <= y)
    {
        string foundTaxiID = getTaxiAtPos(X, Y);
        if (foundTaxiID == null || foundTaxiID.Length <= 0)
        {
            map[X - 1, Y - 1] = newID;
            taxiList.Add(newID);
        }
    }
}

void deleteTaxi(int X, int Y)
{
    string foundTaxiID = getTaxiAtPos(X, Y);
    if (foundTaxiID != null && foundTaxiID.Length > 0)
    {
        map[X - 1, Y - 1] = "";
        int taxiindex = taxiList.IndexOf(foundTaxiID);
        if (taxiindex != -1)
        {
            taxiList.RemoveAt(taxiindex);
        }
    }
}

// алгоритмы поиска

// поиск по списку такси
string[] getClosestTaxis1(int X, int Y)
{
    int i = 0;
    string[] taxis = new string[searchAmount];
    int[] taxiPos = { -1, -1 };
    double curDist = x * y;
    Dictionary<string, double> taxiDist = new Dictionary<string, double>();
    foreach (string taxiID in taxiList)
    {
        taxiPos = getTaxiPos(taxiID);
        if (taxiPos[0] > 0 && taxiPos[1] > 0)
        {
            curDist = getDist(X + 1, Y + 1, taxiPos[0], taxiPos[1]);
            taxiDist.Add(taxiID, curDist);
        }
    }
    taxiDist = taxiDist.OrderBy(i => i.Value).ToDictionary(i => i.Key, i => i.Value);
    foreach (KeyValuePair<string, double> kvp in taxiDist)
    {
        if (i == searchAmount) break;
        Console.WriteLine("Taxi = {0}, Dist = {1}", kvp.Key, kvp.Value);
        taxis[i] = kvp.Key;
        i++;
    }
    return taxis;
}

// поиск по квадрату (ищет ближ. такси в квадрате, размер постепенно увеличивается. Когда размер > 2, то диагональ будет больше чем след. размер. Из-за этого пришлось делать систему защиты, которая будет сканировать след. квадраты до того, пока там не будет ни одной ячейки, у которой дистанция до точки поиска меньше, чем максимальная, когда мы искали первоначально)
string[] getClosestTaxis2(int X, int Y)
{
    int k = 0;
    string[] taxis = new string[searchAmount];
    int forceLast = 2;
    double curDist = x * y;
    double maxDist = 0f;
    Dictionary<string, double> taxiDist = new Dictionary<string, double>();
    string taxi = getTaxiAtPos(X + 1, Y + 1);
    if (taxi != null && taxi != "" && taxi.Length > 0)
    {
        taxiDist.Add(taxi, 0);
    }
    int[] curPos = { X, Y };
    for (int i = 1; i < Math.Max(x, y) + 1; i++)
    {
        if (forceLast > 0)
        {
            if (forceLast == 1) forceLast = 0;
            int[][] points = getPointsInSquare(X + 1, Y + 1, i);
            foreach (int[] point in points)
            {
                if (pointExists(point[0], point[1]))
                {
                    taxi = getTaxiAtPos(point[0], point[1]);
                    if (taxi != null && taxi != "" && taxi.Length > 0)
                    {
                        //Console.WriteLine(Convert.ToString(X+1)+","+ Convert.ToString(Y+1)+","+ Convert.ToString(point[0])+","+ Convert.ToString(point[1])); 💀
                        curDist = getDist(X + 1, Y + 1, point[0], point[1]);
                        if (forceLast == 0 && curDist < maxDist) forceLast = 1;
                        else if (forceLast == 2) maxDist = Math.Max(maxDist, curDist);
                        taxiDist.Add(taxi, curDist);
                        k++;
                    }
                }

            }
        }
        else
        {
            break;
        }
        if (k >= searchAmount)
        {
            forceLast = 1;
        }
    }
    k = 0;
    taxiDist = taxiDist.OrderBy(k => k.Value).ToDictionary(k => k.Key, k => k.Value);
    foreach (KeyValuePair<string, double> kvp in taxiDist)
    {
        if (k == searchAmount) break;
        Console.WriteLine("Taxi = {0}, Dist = {1}", kvp.Key, kvp.Value);
        taxis[k] = kvp.Key;
        k++;
    }
    return taxis;
}

// этот метод будет искать такси в радиусе от точки запроса. Радиус будет постепенно увеличиваться, пока машины не будут найдены или радиус будет слишком большой (больше, чем длина/ширина карты)
string[] getClosestTaxis3(int X, int Y)
{
    int k = 0;
    string[] taxis = new string[searchAmount];
    Dictionary<string, double> taxiDist = new Dictionary<string, double>();
    string taxi = getTaxiAtPos(X + 1, Y + 1);
    if (taxi != null && taxi != "" && taxi.Length > 0)
    {
        taxiDist.Add(taxi, 0);
    }
    for (int i = 1; i < Convert.ToInt32(Math.Ceiling(Math.Sqrt(x * x + y * y))); i++)
    {
        int[][] points = getPointsInCircle(X + 1, Y + 1, i + 0.001, i - 0.999);
        foreach (int[] point in points)
        {
            if (point != null && pointExists(point[0], point[1]))
            {
                taxi = getTaxiAtPos(point[0], point[1]);
                if (taxi != null && taxi != "" && taxi.Length > 0)
                {
                    taxiDist.Add(taxi, getDist(X + 1, Y + 1, point[0], point[1]));
                }
            }
        }
        if (taxiDist.Count >= searchAmount) break;
    }
    taxiDist = taxiDist.OrderBy(k => k.Value).ToDictionary(k => k.Key, k => k.Value);
    foreach (KeyValuePair<string, double> kvp in taxiDist)
    {
        if (k == searchAmount) break;
        Console.WriteLine("Taxi = {0}, Dist = {1}", kvp.Key, kvp.Value);
        taxis[k] = kvp.Key;
        k++;
    }
    return taxis;
}

int g = 0;
int h = 0;
string name;

// ручная инициализация такси
/*
for (int i = 0; i < n; i++)
{
    Console.WriteLine("Введите координату X такси № " + Convert.ToString(i + 1) + ": ");
    g = Convert.ToInt32(Console.ReadLine());
    if (g <= 0 || g > x)
    {
        Console.WriteLine("Ошибка! Координата X такси должна быть больше 0 и меньше " + Convert.ToString(x));
        return;
    }
    Console.WriteLine("Введите координату Y такси № " + Convert.ToString(i + 1) + ": ");
    h = Convert.ToInt32(Console.ReadLine());
    if (h <= 0 || h > y)
    {
        Console.WriteLine("Ошибка! Координата X такси должна быть больше 0 и меньше " + Convert.ToString(x));
        return;
    }
    string foundTaxiID = getTaxiAtPos(g, h);
    if (foundTaxiID != null && foundTaxiID.Length > 0)
    {
        Console.WriteLine("Ошибка! Такси на таких координатах уже существует!");
        return;
    }
    Console.WriteLine("Введите ID такси № " + Convert.ToString(i + 1) + ": ");
    name = Console.ReadLine();
    if (alreadyExists(name))
    {
        Console.WriteLine("Ошибка! Такси с таким ID уже существует!");
        return;
    }
    map[g - 1, h - 1] = name;
    taxiList.Add(name);
}
*/

// рандомная инициализация такси
int i = 0;
while (i < n)
{
    g = random.Next(1, x + 1);
    h = random.Next(1, y + 1);
    string foundTaxiID = getTaxiAtPos(g, h);
    if (foundTaxiID != null && foundTaxiID.Length > 0)
    {
        Console.WriteLine("Ошибка! Такси на таких координатах уже существует! (" + Convert.ToString(i) + ")");
    }
    else
    {
        name = Convert.ToString(g) + "," + Convert.ToString(h);
        map[g - 1, h - 1] = name;
        taxiList.Add(name);
        i++;
    }
}

// инициализация точки поиска:

Console.WriteLine("Введите координату X точки поиска");
g = Convert.ToInt32(Console.ReadLine());
if (g <= 0 || g > x)
{
    Console.WriteLine("Ошибка! Координата X точки поиска должна быть больше 0 и меньше " + Convert.ToString(x));
    return;
}
Console.WriteLine("Введите координату Y точки поиска");
h = Convert.ToInt32(Console.ReadLine());
if (h <= 0 || h > y)
{
    Console.WriteLine("Ошибка! Координата X точки поиска должна быть больше 0 и меньше " + Convert.ToString(x));
    return;
}

getClosestTaxis1(g - 1, h - 1);
Console.WriteLine("----------------------------");
getClosestTaxis2(g - 1, h - 1);
Console.WriteLine("----------------------------");
getClosestTaxis3(g - 1, h - 1);

/*
g = random.Next(1, x + 1);
h = random.Next(1, y + 1);
*/


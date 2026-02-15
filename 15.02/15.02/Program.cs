using System;

namespace Массив_студенты
{
    class Студент
    {
        public string ФИО { get; set; }
        public string Группа { get; set; }
        public double Балл { get; set; }


        public Студент(string фио, string группа, double балл)
        {
            ФИО = фио;
            Группа = группа;
            Балл = балл;
        }

        public void Показать()
        {
            Console.WriteLine($"ФИО: {ФИО,-25}, Группа: {Группа,-8}, Балл: {Балл,5:F1}");
        }


        public string Оценка()
        {
            if (Балл == 5) return "Отлично";
            else if (Балл == 4) return "Хорошо";
            else if (Балл == 3) return "Удовлетворительно";
            else if (Балл == 2) return "Неаттестация";
            else if (Балл == 1) return "Неаттестация";
            else if (Балл == 0) return "У студента нет оценок";
            else return "Введено некоректное число";
        }




        static void Main()
        {
            Console.Write("Введите количество студентов: ");
            int count = int.Parse(Console.ReadLine());

            // Создаем массив нужного размера
            Студент[] студенты = new Студент[count];

            for (int i = 0; i < студенты.Length; i++)
            {
                Console.WriteLine($"\nВведите данные для студента {i + 1}:");

                Console.Write("ФИО: ");
                string fullName = Console.ReadLine();

                Console.Write("Группа: ");
                string group = Console.ReadLine();

                Console.Write("Балл: ");
                double score = double.Parse(Console.ReadLine());

                студенты[i] = new Студент(fullName, group, score);
            }

            // Выводим всех студентов
            Console.WriteLine("\n" + new string('-', 60));
            Console.WriteLine("Список студентов:");
            Console.WriteLine(new string('-', 60));

            foreach (var студент in студенты)
            {
                студент.Показать();
            }

            // МЕНЮ СОРТИРОВКИ
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("СОРТИРОВКА ПО БАЛЛАМ");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("1 - Сортировка по возрастанию баллов");
            Console.WriteLine("2 - Сортировка по убыванию баллов");
            Console.WriteLine("0 - Выйти без сортировки");
            Console.Write("Выберите опцию: ");

            string выбор = Console.ReadLine();


            if (выбор == "1" || выбор == "2")
            {
                // Создаем копию массива для сортировки
                Студент[] отсортированные = new Студент[студенты.Length];
                Array.Copy(студенты, отсортированные, студенты.Length);

                // Сортируем
                if (выбор == "1")
                {
                    // По возрастанию
                    Array.Sort(отсортированные, (a, b) => a.Балл.CompareTo(b.Балл));
                    Console.WriteLine("\nСортировка по ВОЗРАСТАНИЮ баллов:");
                }
                else
                {
                    // По убыванию
                    Array.Sort(отсортированные, (a, b) => b.Балл.CompareTo(a.Балл));
                    Console.WriteLine("\nСортировка по УБЫВАНИЮ баллов:");
                }

                Console.WriteLine(new string('-', 60));

                // Выводим отсортированный список
                for (int i = 0; i < отсортированные.Length; i++)
                {
                    Console.Write($"{i + 1,2}. ");
                    отсортированные[i].Показать();
                }

                // Дополнительная информация
                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"Лучший результат: {отсортированные[0].ФИО} - {отсортированные[0].Балл:F1} ({отсортированные[0].Оценка()})");
                Console.WriteLine($"Худший результат: {отсортированные[отсортированные.Length - 1].ФИО} - {отсортированные[отсортированные.Length - 1].Балл:F1} ({отсортированные[отсортированные.Length - 1].Оценка()})");
            }
            else
            {
                Console.WriteLine("\nСортировка не выполнена.");
            }

            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("Программа завершена. Нажмите любую клавишу...");
            Console.ReadKey();



        }






        

    }
}
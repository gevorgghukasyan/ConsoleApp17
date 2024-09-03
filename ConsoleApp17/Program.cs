using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp17
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Repository<Car, Guid> repository = new Repository<Car, Guid>();

            repository.Add(new Car(Guid.NewGuid()) { Color = "Black", Mark = "BMW" });
            repository.Add(new Car(Guid.NewGuid()) { Color = "Yellow", Mark = "Tesla" });
            repository.Add(new Car(Guid.NewGuid()) { Color = "White", Mark = "Mercedes" });

            var r = repository.GetAll().ToList();

            r.Filter(x => x.Mark == "BMW");
            r.Filter(x => x.Color == "Yellow");
            r.Filter<Car>(x => x.Mark == "Mercedes");

            repository.Update(new Car(r[0].Key) { Color = "White", Mark = "Nissan" }, r[0].Key);

            List<Person> people = new List<Person>()
            {
                new Person(){ Id = 30},
                new Person(){ Id = 15},
                new Person(){ Id = 20},
            };

            people.SortPerson().ForEach(x => Console.WriteLine(x.Id));
            people.Sort(x => x.Id);
        }
    }

    public class Car : EntityBase<Guid>
    {
        public string Mark { get; set; }
        public string Color { get; set; }
        public int Year { get; set; }

        public Car(Guid key) : base(key)
        {

        }
    }

    public interface IRepository<TEntity, TKey> where TEntity : EntityBase<TKey> where TKey : struct
    {
        void Add(TEntity entity);
        void Remove(TKey key);
        IEnumerable<TEntity> GetAll();
        void Update(TEntity entity, TKey key);
        TEntity GetById(TKey key);
    }

    public class EntityBase<TKey> where TKey : struct
    {
        public TKey Key { get; set; }

        public EntityBase(TKey key)
        {
            Key = key;
        }
    }

    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : EntityBase<TKey> where TKey : struct
    {
        List<TEntity> collection = new List<TEntity>();

        public void Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new Exception("Parameter can't be null.");
            }

            collection.Add(entity);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return collection;
        }

        public TEntity GetById(TKey key)
        {
            return collection.FirstOrDefault(x => x.Key.Equals(key));
        }

        public void Remove(TKey key)
        {
            var entity = GetById(key);

            if (entity == null)
            {
                throw new Exception($"Can't find item with key {key}");
            }

            collection.Remove(entity);
        }

        public void Update(TEntity entity, TKey key)
        {
            var e = GetById(key);

            if (e == null)
            {
                throw new Exception($"Can't find item with key {key}");
            }

            int index = collection.IndexOf(e);

            collection[index] = entity;
        }
    }

    class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
    }

    public delegate bool Where<T>(T t);

    static class Extentions
    {
        public static List<T> Filter<T>(this List<T> cars, Where<T> condition)
        {
            List<T> filteredCars = new List<T>();

            foreach (var car in cars)
            {
                if (condition(car))
                {
                    filteredCars.Add(car);
                }
            }

            return filteredCars;
        }

        public static List<Person> SortPerson(this List<Person> people)
        {
            return people.OrderBy(x => x.Name).ToList();
        }

        public static List<TSource> Sort<TSource, TKey>(this List<TSource> values, Func<TSource, TKey> func)
        {
            return values.OrderBy(func).ToList();
        }
    }

    /*Exercise 2: Generic Repository Pattern
	Objective: Implement a generic repository pattern to manage a collection of items.
	Requirements:
	Generics
	Interfaces
	Extension Methods
	Description:
	Create a generic interface IRepository<T> with methods like Add, Remove, and GetAll.
	Implement a class Repository<T> that implements IRepository<T>.
	Add extension methods to filter and sort items in the repository.*/
}

namespace DependencyInjectionWorkshop.Models
{
    public class Member
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        //Alt + Ins -> formatting Members
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Age)}: {Age}";
        }
    }
}
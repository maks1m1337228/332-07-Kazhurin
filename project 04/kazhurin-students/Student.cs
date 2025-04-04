using System;
using System.ComponentModel.DataAnnotations;

namespace project_04
{
    public class Student
    {
        [Required(ErrorMessage = "Фамилия обязательна для заполнения")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Имя обязательно для заполнения")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Отчество обязательно для заполнения")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Курс обязателен для заполнения")]
        public int Course { get; set; }

        [Required(ErrorMessage = "Группа обязательна для заполнения")]
        public string Group { get; set; }

        [Required(ErrorMessage = "Дата рождения обязательна для заполнения")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Email обязателен для заполнения")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; }
    }
} 
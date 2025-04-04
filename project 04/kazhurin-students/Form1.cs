using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace project_04
{
    public partial class Form1 : Form
    {
        private List<Student> students = new List<Student>();
        private bool isDataChanged = false;
        private string currentFilePath = "students.json";

        public Form1()
        {
            InitializeComponent();
            LoadData();
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LastName",
                HeaderText = "Фамилия",
                Name = "LastName"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FirstName",
                HeaderText = "Имя",
                Name = "FirstName"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MiddleName",
                HeaderText = "Отчество",
                Name = "MiddleName"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Course",
                HeaderText = "Курс",
                Name = "Course"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Group",
                HeaderText = "Группа",
                Name = "Group"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "BirthDate",
                HeaderText = "Дата рождения",
                Name = "BirthDate"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Email",
                HeaderText = "Email",
                Name = "Email"
            });
        }

        private void LoadData()
        {
            if (File.Exists(currentFilePath))
            {
                string json = File.ReadAllText(currentFilePath);
                students = JsonConvert.DeserializeObject<List<Student>>(json) ?? new List<Student>();
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = students;
            }
        }

        private void SaveData()
        {
            string json = JsonConvert.SerializeObject(students, Formatting.Indented);
            File.WriteAllText(currentFilePath, json);
            isDataChanged = false;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            
            string[] allowedDomains = { "yandex.ru", "gmail.com", "icloud.com" };
            string pattern = @"^[a-zA-Z0-9._%+-]{3,}@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            
            if (!Regex.IsMatch(email, pattern)) return false;
            
            string domain = email.Split('@')[1].ToLower();
            return allowedDomains.Contains(domain);
        }

        private bool ValidateBirthDate(DateTime date)
        {
            return date >= new DateTime(1992, 1, 1) && date <= DateTime.Now;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtMiddleName.Text) ||
                string.IsNullOrWhiteSpace(txtGroup.Text) ||
                !ValidateEmail(txtEmail.Text) ||
                !ValidateBirthDate(dateTimePicker1.Value))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var student = new Student
            {
                LastName = txtLastName.Text,
                FirstName = txtFirstName.Text,
                MiddleName = txtMiddleName.Text,
                Course = (int)numCourse.Value,
                Group = txtGroup.Text,
                BirthDate = dateTimePicker1.Value,
                Email = txtEmail.Text
            };

            students.Add(student);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = students;
            isDataChanged = true;
            ClearFields();
        }

        private void ClearFields()
        {
            txtLastName.Clear();
            txtFirstName.Clear();
            txtMiddleName.Clear();
            numCourse.Value = 1;
            txtGroup.Clear();
            dateTimePicker1.Value = DateTime.Now;
            txtEmail.Clear();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var student = dataGridView1.SelectedRows[0].DataBoundItem as Student;
                if (student != null)
                {
                    students.Remove(student);
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = students;
                    isDataChanged = true;
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var student = dataGridView1.SelectedRows[0].DataBoundItem as Student;
                if (student != null)
                {
                    txtLastName.Text = student.LastName;
                    txtFirstName.Text = student.FirstName;
                    txtMiddleName.Text = student.MiddleName;
                    numCourse.Value = student.Course;
                    txtGroup.Text = student.Group;
                    dateTimePicker1.Value = student.BirthDate;
                    txtEmail.Text = student.Email;

                    students.Remove(student);
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = students;
                    isDataChanged = true;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveData();
            MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isDataChanged)
            {
                var result = MessageBox.Show("Есть несохраненные изменения. Хотите сохранить?", "Предупреждение",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveData();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();
            var filteredStudents = students.Where(s => s.LastName.ToLower().Contains(searchText)).ToList();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = filteredStudents;
        }

        private void cmbFilterCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void cmbFilterGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filteredStudents = students.AsQueryable();

            if (cmbFilterCourse.SelectedItem != null)
            {
                int course = (int)cmbFilterCourse.SelectedItem;
                filteredStudents = filteredStudents.Where(s => s.Course == course);
            }

            if (cmbFilterGroup.SelectedItem != null)
            {
                string group = cmbFilterGroup.SelectedItem.ToString();
                filteredStudents = filteredStudents.Where(s => s.Group == group);
            }

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = filteredStudents.ToList();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
                saveFileDialog.FilterIndex = 1;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8))
                    {
                        sw.WriteLine("Фамилия,Имя,Отчество,Курс,Группа,Дата рождения,Email");
                        foreach (var student in students)
                        {
                            sw.WriteLine($"{student.LastName},{student.FirstName},{student.MiddleName}," +
                                       $"{student.Course},{student.Group},{student.BirthDate:dd.MM.yyyy},{student.Email}");
                        }
                    }
                    MessageBox.Show("Данные успешно экспортированы", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var importedStudents = new List<Student>();
                        var lines = File.ReadAllLines(openFileDialog.FileName, Encoding.UTF8);
                        
                        for (int i = 1; i < lines.Length; i++)
                        {
                            var values = lines[i].Split(',');
                            if (values.Length >= 7)
                            {
                                var student = new Student
                                {
                                    LastName = values[0],
                                    FirstName = values[1],
                                    MiddleName = values[2],
                                    Course = int.Parse(values[3]),
                                    Group = values[4],
                                    BirthDate = DateTime.ParseExact(values[5], "dd.MM.yyyy", null),
                                    Email = values[6]
                                };
                                importedStudents.Add(student);
                            }
                        }

                        students.AddRange(importedStudents);
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = students;
                        isDataChanged = true;
                        MessageBox.Show("Данные успешно импортированы", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при импорте данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

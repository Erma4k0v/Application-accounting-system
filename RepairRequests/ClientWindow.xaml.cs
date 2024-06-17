using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RepairRequests
{
    public partial class ClientWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Request> _requests;
        public ObservableCollection<Request> Requests
        {
            get { return _requests; }
            set
            {
                _requests = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Requests)));
            }
        }

        const string connectionString = @"Server=DESKTOP-1DTLLG9\SQLEXPRESS;Database=RepairRequestsDB; Integrated Security=True; TrustServerCertificate=True";
        private int clientId; // Уберем значение по умолчанию и будем использовать параметр конструктора

        private Dictionary<string, string> lastKnownStatuses;

        public ClientWindow(int userId)
        {
            InitializeComponent();
            DataContext = this;
            this.clientId = userId;
            Requests = new ObservableCollection<Request>();

            // Загружаем предыдущие статусы заявок из файла
            lastKnownStatuses = LoadLastKnownStatuses();

            // При запуске приложения загружаем заявки клиента из базы данных
            LoadClientRequests();
        }

        // Метод для загрузки заявок клиента из базы данных
        private async void LoadClientRequests()
        {
            // Загружаем предыдущие статусы заявок из файла
            lastKnownStatuses = LoadLastKnownStatuses();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Подготавливаем SQL-запрос для загрузки заявок клиента
                string selectQuery = @"SELECT R.RequestNumber, R.DateAdded, R.Equipment, R.FaultType, R.ProblemDescription, R.Status, R.Comment
                               FROM Requests R
                               WHERE R.ClientID = @ClientId";

                // Создаем команду для выполнения запроса
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    // Добавляем параметр с ID клиента
                    command.Parameters.AddWithValue("@ClientId", clientId);

                    // Выполняем запрос и читаем результаты
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Requests.Clear();
                        while (reader.Read())
                        {
                            var requestNumber = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                            var dateAdded = reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1);
                            var equipment = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                            var faultType = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            var problemDescription = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                            var status = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                            var comment = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);

                            // Создаем новый объект заявки и добавляем его в коллекцию для отображения
                            Request request = new Request
                            {
                                RequestNumber = requestNumber,
                                DateAdded = dateAdded,
                                Equipment = equipment,
                                FaultType = faultType,
                                ProblemDescription = problemDescription,
                                Status = status,
                                Comments = comment // Добавляем комментарий к заявке
                            };

                            // Проверяем изменения статуса и уведомляем клиента
                            if (lastKnownStatuses.ContainsKey(requestNumber))
                            {
                                if (lastKnownStatuses[requestNumber] != status)
                                {
                                    await Task.Delay(1000); // Задержка на 1 секунду перед показом MessageBox
                                    MessageBox.Show($"Статус заявки {requestNumber} изменен на {status}.");
                                }

                                if (status == "Завершено" && lastKnownStatuses[requestNumber] != "Завершено")
                                {
                                    await Task.Delay(1000); // Задержка на 1 секунду перед показом MessageBox
                                    MessageBox.Show($"Работа по заявке {requestNumber} завершена.");
                                }
                            }

                            lastKnownStatuses[requestNumber] = status;

                            Requests.Add(request);
                        }
                    }
                }
            }

            // Сохраняем текущие статусы заявок в файл
            SaveLastKnownStatuses(lastKnownStatuses);
        }

        // Метод для сохранения текущих статусов заявок в файл
        private void SaveLastKnownStatuses(Dictionary<string, string> statuses)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(statuses, options);
            File.WriteAllText("lastKnownStatuses.json", jsonString);
        }

        // Метод для загрузки предыдущих статусов заявок из файла
        private Dictionary<string, string> LoadLastKnownStatuses()
        {
            if (File.Exists("lastKnownStatuses.json"))
            {
                string jsonString = File.ReadAllText("lastKnownStatuses.json");
                return JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
            }
            return new Dictionary<string, string>();
        }

        private void AddRequest_Click(object sender, RoutedEventArgs e)
        {
            string requestNumber = txtRequestNumber.Text;
            string equipment = txtEquipment.Text;
            string faultType = txtFaultType.Text;
            string problemDescription = txtProblemDescription.Text;
            DateTime dateAdded = DateTime.Now;
            string status = "В ожидании"; // Устанавливаем начальный статус

            // Создаем подключение к базе данных
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Подготавливаем SQL-запрос для вставки новой заявки
                string insertQuery = @"INSERT INTO Requests (RequestNumber, DateAdded, Equipment, FaultType, ProblemDescription, ClientID, Status)
                               VALUES (@RequestNumber, @DateAdded, @Equipment, @FaultType, @ProblemDescription, @ClientID, @RequestStatus)";

                // Создаем команду для выполнения запроса
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    // Добавляем параметры к запросу
                    command.Parameters.AddWithValue("@RequestNumber", requestNumber);
                    command.Parameters.AddWithValue("@DateAdded", dateAdded);
                    command.Parameters.AddWithValue("@Equipment", equipment);
                    command.Parameters.AddWithValue("@FaultType", faultType);
                    command.Parameters.AddWithValue("@ProblemDescription", problemDescription);
                    command.Parameters.AddWithValue("@ClientID", clientId);
                    command.Parameters.AddWithValue("@RequestStatus", status); // Переименовали параметр

                    // Выполняем запрос
                    command.ExecuteNonQuery();
                }
            }

            // Создаем новый объект заявки и добавляем его в коллекцию для отображения на интерфейсе
            Request newRequest = new Request
            {
                RequestNumber = requestNumber,
                Equipment = equipment,
                FaultType = faultType,
                ProblemDescription = problemDescription,
                DateAdded = dateAdded,
                Status = status // Устанавливаем статус
            };

            Requests.Add(newRequest);
        }

        private void ChangeProblemDescription_Click(object sender, RoutedEventArgs e)
        {
            string requestNumber = txtRequestNumberToUpdate.Text;
            string newProblemDescription = txtNewProblemDescription.Text;

            if (string.IsNullOrEmpty(requestNumber) || string.IsNullOrEmpty(newProblemDescription))
            {
                MessageBox.Show("Пожалуйста, введите номер заявки и новое описание проблемы.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Подготавливаем SQL-запрос для обновления описания проблемы
                string updateQuery = "UPDATE Requests SET ProblemDescription = @NewProblemDescription WHERE RequestNumber = @RequestNumber AND ClientID = @ClientId";

                // Создаем команду для выполнения запроса
                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@NewProblemDescription", newProblemDescription);
                    command.Parameters.AddWithValue("@RequestNumber", requestNumber);
                    command.Parameters.AddWithValue("@ClientId", clientId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Описание проблемы успешно обновлено.");

                        // Обновляем описание проблемы в коллекции Requests
                        var request = Requests.FirstOrDefault(r => r.RequestNumber == requestNumber);
                        if (request != null)
                        {
                            request.ProblemDescription = newProblemDescription;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Заявка с данным номером не найдена.");
                    }
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == textBox.Tag.ToString())
            {
                textBox.Text = "";
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Tag.ToString();
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        public class Request : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string _requestNumber;
            public string RequestNumber
            {
                get { return _requestNumber; }
                set
                {
                    _requestNumber = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RequestNumber)));
                }
            }

            private DateTime _dateAdded;
            public DateTime DateAdded
            {
                get { return _dateAdded; }
                set
                {
                    _dateAdded = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DateAdded)));
                }
            }

            private string _equipment;
            public string Equipment
            {
                get { return _equipment; }
                set
                {
                    _equipment = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Equipment)));
                }
            }

            private string _faultType;
            public string FaultType
            {
                get { return _faultType; }
                set
                {
                    _faultType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FaultType)));
                }
            }

            private string _problemDescription;
            public string ProblemDescription
            {
                get { return _problemDescription; }
                set
                {
                    _problemDescription = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProblemDescription)));
                }
            }

            private string _status;
            public string Status
            {
                get { return _status; }
                set
                {
                    _status = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                }
            }

            private string _comments;
            public string Comments
            {
                get { return _comments; }
                set
                {
                    _comments = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Comments)));
                }
            }
        }
    }
}

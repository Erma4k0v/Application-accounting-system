using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace RepairRequests
{
    public partial class AdminWindow : Window, INotifyPropertyChanged
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

        public AdminWindow()
        {
            InitializeComponent();
            DataContext = this;
            Requests = new ObservableCollection<Request>();

            // Загрузка заявок из базы данных
            LoadRequests();
            // Загрузка статистики
            LoadStatistics();
        }

        // Метод для загрузки заявок из базы данных
        private void LoadRequests()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectQuery = @"SELECT R.RequestNumber, R.DateAdded, R.Equipment, R.FaultType, R.ProblemDescription, U.UserName AS ClientName, R.Status
                               FROM Requests R
                               JOIN Users U ON R.ClientID = U.UserID";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
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
                            var clientName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                            var status = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);

                            Request request = new Request
                            {
                                RequestNumber = requestNumber,
                                DateAdded = dateAdded,
                                Equipment = equipment,
                                FaultType = faultType,
                                ProblemDescription = problemDescription,
                                ClientName = clientName,
                                Status = status
                            };

                            Requests.Add(request);
                        }
                    }
                }
            }
        }

        private void AddExecutor_Click(object sender, RoutedEventArgs e)
        {
            string executorLogin = txtExecutorLogin.Text;
            string requestNumber = txtRequestNumberForExecutor.Text;

            if (string.IsNullOrEmpty(executorLogin) || string.IsNullOrEmpty(requestNumber))
            {
                MessageBox.Show("Пожалуйста, введите логин мастера и номер заявки.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Проверяем, существует ли мастер с данным логином
                string selectMasterQuery = "SELECT UserID FROM Users WHERE UserLogin = @ExecutorLogin AND Role = 'Master'";
                SqlCommand selectMasterCommand = new SqlCommand(selectMasterQuery, connection);
                selectMasterCommand.Parameters.AddWithValue("@ExecutorLogin", executorLogin);

                object masterIdObj = selectMasterCommand.ExecuteScalar();
                if (masterIdObj == null)
                {
                    MessageBox.Show("Мастер с данным логином не найден.");
                    return;
                }

                int masterId = (int)masterIdObj;

                // Обновляем заявку, назначая мастера
                string updateRequestQuery = "UPDATE Requests SET MasterID = @MasterID WHERE RequestNumber = @RequestNumber";
                SqlCommand updateRequestCommand = new SqlCommand(updateRequestQuery, connection);
                updateRequestCommand.Parameters.AddWithValue("@MasterID", masterId);
                updateRequestCommand.Parameters.AddWithValue("@RequestNumber", requestNumber);

                int rowsAffected = updateRequestCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Мастер успешно назначен на заявку.");
                    LoadRequests();
                }
                else
                {
                    MessageBox.Show("Заявка с данным номером не найдена.");
                }
            }
        }

        private void ChangeResponsible_Click(object sender, RoutedEventArgs e)
        {
            string executorLogin = txtExecutorLogin.Text;
            string requestNumber = txtRequestNumberForExecutor.Text;

            if (string.IsNullOrEmpty(executorLogin) || string.IsNullOrEmpty(requestNumber))
            {
                MessageBox.Show("Пожалуйста, введите логин нового мастера и номер заявки.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Проверяем, существует ли мастер с данным логином
                string selectMasterQuery = "SELECT UserID FROM Users WHERE UserLogin = @ExecutorLogin AND Role = 'Master'";
                SqlCommand selectMasterCommand = new SqlCommand(selectMasterQuery, connection);
                selectMasterCommand.Parameters.AddWithValue("@ExecutorLogin", executorLogin);

                object masterIdObj = selectMasterCommand.ExecuteScalar();
                if (masterIdObj == null)
                {
                    MessageBox.Show("Мастер с данным логином не найден.");
                    return;
                }

                int masterId = (int)masterIdObj;

                // Обновляем заявку, назначая нового мастера
                string updateRequestQuery = "UPDATE Requests SET MasterID = @MasterID WHERE RequestNumber = @RequestNumber";
                SqlCommand updateRequestCommand = new SqlCommand(updateRequestQuery, connection);
                updateRequestCommand.Parameters.AddWithValue("@MasterID", masterId);
                updateRequestCommand.Parameters.AddWithValue("@RequestNumber", requestNumber);

                int rowsAffected = updateRequestCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Изменение ответственного за заявку успешно выполнено.");
                    LoadRequests();
                }
                else
                {
                    MessageBox.Show("Заявка с данным номером не найдена.");
                }
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string requestNumber = txtSearchRequestNumber.Text == "Номер заявки" ? string.Empty : txtSearchRequestNumber.Text;
            string equipment = txtSearchEquipment.Text == "Оборудование" ? string.Empty : txtSearchEquipment.Text;
            string faultType = txtSearchFaultType.Text == "Тип неисправности" ? string.Empty : txtSearchFaultType.Text;
            string clientName = txtSearchClientName.Text == "Имя клиента" ? string.Empty : txtSearchClientName.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectQuery = @"SELECT R.RequestNumber, R.DateAdded, R.Equipment, R.FaultType, R.ProblemDescription, U.UserName AS ClientName, R.Status
                                       FROM Requests R
                                       JOIN Users U ON R.ClientID = U.UserID
                                       WHERE (@RequestNumber IS NULL OR R.RequestNumber LIKE '%' + @RequestNumber + '%')
                                         AND (@Equipment IS NULL OR R.Equipment LIKE '%' + @Equipment + '%')
                                         AND (@FaultType IS NULL OR R.FaultType LIKE '%' + @FaultType + '%')
                                         AND (@ClientName IS NULL OR U.UserName LIKE '%' + @ClientName + '%')";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@RequestNumber", string.IsNullOrEmpty(requestNumber) ? (object)DBNull.Value : requestNumber);
                    command.Parameters.AddWithValue("@Equipment", string.IsNullOrEmpty(equipment) ? (object)DBNull.Value : equipment);
                    command.Parameters.AddWithValue("@FaultType", string.IsNullOrEmpty(faultType) ? (object)DBNull.Value : faultType);
                    command.Parameters.AddWithValue("@ClientName", string.IsNullOrEmpty(clientName) ? (object)DBNull.Value : clientName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Requests.Clear();
                        while (reader.Read())
                        {
                            var reqNumber = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                            var dateAdded = reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1);
                            var equip = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                            var fault = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            var problemDesc = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                            var client = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                            var status = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);

                            Request request = new Request
                            {
                                RequestNumber = reqNumber,
                                DateAdded = dateAdded,
                                Equipment = equip,
                                FaultType = fault,
                                ProblemDescription = problemDesc,
                                ClientName = client,
                                Status = status
                            };

                            Requests.Add(request);
                        }
                    }
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == textBox.Tag.ToString())
            {
                textBox.Text = string.Empty;
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
        private void LoadStatistics()
        {
            int completedRequestsCount = 0;
            double totalExecutionTime = 0;
            Dictionary<string, int> faultTypeCounts = new Dictionary<string, int>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Получение статистики по выполненным заявкам
                string selectQuery = @"SELECT COUNT(*) FROM Requests WHERE Status = 'Выполнено'";
                SqlCommand command = new SqlCommand(selectQuery, connection);
                completedRequestsCount = (int)command.ExecuteScalar();

                // Получение общего времени выполнения заявок
                selectQuery = @"SELECT AVG(DATEDIFF(HOUR, DateAdded, GETDATE())) FROM Requests WHERE Status = 'Выполнено'";
                command = new SqlCommand(selectQuery, connection);
                object averageExecutionTimeObj = command.ExecuteScalar();
                if (averageExecutionTimeObj != DBNull.Value)
                {
                    totalExecutionTime = Convert.ToDouble(averageExecutionTimeObj);
                }

                // Получение статистики по типам неисправностей
                selectQuery = @"SELECT FaultType, COUNT(*) FROM Requests GROUP BY FaultType";
                command = new SqlCommand(selectQuery, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string faultType = reader.GetString(0);
                        int count = reader.GetInt32(1);
                        faultTypeCounts.Add(faultType, count);
                    }
                }
            }

            // Обновление TextBlock с загруженной статистикой
            txtCompletedRequests.Text = $"Выполненные заявки: {completedRequestsCount}";
            txtAverageExecutionTime.Text = $"Среднее время выполнения заявки: {totalExecutionTime} часов";
            txtFaultTypeStatistics.Text = "Статистика по типам неисправностей:";
            foreach (var pair in faultTypeCounts)
            {
                txtFaultTypeStatistics.Text += $"\n{pair.Key}: {pair.Value}";
            }
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

        private string _clientName;
        public string ClientName
        {
            get { return _clientName; }
            set
            {
                _clientName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClientName)));
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
    }
}

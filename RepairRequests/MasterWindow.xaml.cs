using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace RepairRequests
{
    public partial class MasterWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Request> _assignedRequests;
        public ObservableCollection<Request> AssignedRequests
        {
            get { return _assignedRequests; }
            set
            {
                _assignedRequests = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AssignedRequests)));
            }
        }

        private string _masterLogin;
        public string MasterLogin
        {
            get { return _masterLogin; }
            set
            {
                _masterLogin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MasterLogin)));
            }
        }

        const string connectionString = @"Server=DESKTOP-1DTLLG9\SQLEXPRESS;Database=RepairRequestsDB; Integrated Security=True; TrustServerCertificate=True";

        public MasterWindow(string masterLogin)
        {
            InitializeComponent();
            DataContext = this;
            MasterLogin = masterLogin;
            AssignedRequests = new ObservableCollection<Request>();

            // Загрузка назначенных заявок
            LoadAssignedRequests();
        }

        private void LoadAssignedRequests()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Получаем ID мастера по логину
                string selectMasterIdQuery = "SELECT UserID FROM Users WHERE UserLogin = @MasterLogin";
                SqlCommand selectMasterIdCommand = new SqlCommand(selectMasterIdQuery, connection);
                selectMasterIdCommand.Parameters.AddWithValue("@MasterLogin", MasterLogin);

                int masterId = (int)selectMasterIdCommand.ExecuteScalar();

                // Очищаем список назначенных заявок перед его заполнением
                AssignedRequests.Clear();

                // Получаем заявки, назначенные этому мастеру
                string selectQuery = @"SELECT R.RequestNumber, R.DateAdded, R.Equipment, R.FaultType, R.ProblemDescription, U.UserName AS ClientName, R.Status, R.Comment
                           FROM Requests R
                           JOIN Users U ON R.ClientID = U.UserID
                           WHERE R.MasterID = @MasterID";

                SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                selectCommand.Parameters.AddWithValue("@MasterID", masterId);

                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Request request = new Request
                        {
                            RequestNumber = reader.GetString(0),
                            DateAdded = reader.GetDateTime(1),
                            Equipment = reader.GetString(2),
                            FaultType = reader.GetString(3),
                            ProblemDescription = reader.GetString(4),
                            ClientName = reader.GetString(5),
                            Status = reader.IsDBNull(6) ? "Неизвестный статус" : reader.GetString(6),
                            Comments = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                        };

                        AssignedRequests.Add(request);
                    }
                }
            }
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string requestNumber = button.Tag as string;

            if (string.IsNullOrEmpty(requestNumber))
            {
                MessageBox.Show("Номер заявки не найден.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Получаем текущий статус заявки
                string selectQuery = "SELECT Status FROM Requests WHERE RequestNumber = @RequestNumber";
                SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                selectCommand.Parameters.AddWithValue("@RequestNumber", requestNumber);

                string currentStatus = selectCommand.ExecuteScalar() as string;
                if (string.IsNullOrEmpty(currentStatus))
                {
                    MessageBox.Show("Заявка с данным номером не найдена.");
                    return;
                }

                // Определяем новый статус
                string newStatus;
                switch (currentStatus)
                {
                    case "В ожидании":
                        newStatus = "В работе";
                        break;
                    case "В работе":
                        newStatus = "Выполнено";
                        break;
                    case "Выполнено":
                        MessageBox.Show("Заявка уже выполнена.");
                        return;
                    default:
                        MessageBox.Show("Неизвестный статус заявки.");
                        return;
                }

                // Обновляем статус заявки
                string updateQuery = "UPDATE Requests SET Status = @NewStatus WHERE RequestNumber = @RequestNumber";
                SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("@NewStatus", newStatus);
                updateCommand.Parameters.AddWithValue("@RequestNumber", requestNumber);

                int rowsAffected = updateCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    // Обновляем статус заявки в коллекции для отображения на интерфейсе
                    var request = AssignedRequests.FirstOrDefault(r => r.RequestNumber == requestNumber);
                    if (request != null)
                    {
                        request.Status = newStatus;
                    }
                    MessageBox.Show("Статус заявки успешно обновлен.");
                }
                else
                {
                    MessageBox.Show("Не удалось обновить статус заявки.");
                }
            }
        }

        private void AddComment_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную заявку
            var selectedRequest = (Request)((Button)sender).Tag;

            // Проверяем, выбрана ли заявка
            if (selectedRequest != null)
            {
                // Проверяем, что текст комментария не пустой
                if (!string.IsNullOrEmpty(txtComment.Text))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Добавляем комментарий в базу данных
                        string updateQuery = "UPDATE Requests SET Comment = ISNULL(Comment, '') + @Comment WHERE RequestNumber = @RequestNumber";
                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@Comment", txtComment.Text + Environment.NewLine);
                        updateCommand.Parameters.AddWithValue("@RequestNumber", selectedRequest.RequestNumber);

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Обновляем комментарий в коллекции для отображения на интерфейсе
                            selectedRequest.Comments += txtComment.Text + Environment.NewLine;
                            MessageBox.Show("Комментарий успешно добавлен.");

                            // Обновляем список заявок мастера
                            LoadAssignedRequests();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить комментарий.");
                        }
                    }
                    // Очищаем поле ввода комментария после добавления
                    txtComment.Clear();
                }
                else
                {
                    MessageBox.Show("Пожалуйста, введите текст комментария.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заявку, к которой хотите добавить комментарий.");
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

        private void txtComment_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

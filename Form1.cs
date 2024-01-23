using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Windows.Forms;

namespace CarRental
{
    public partial class Form1 : Form
    {
        private static readonly string connectionString = @"DATA SOURCE=localhost:1521/XE;" + "USER ID=cardb; PASSWORD=carrental";
        private System.Windows.Forms.Timer refreshTimer;
        public Form1()
        {
            InitializeComponent();
            InitializeRefreshTimer();
        }

        private void InitializeRefreshTimer()
        {
            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 5000;
            refreshTimer.Tick += RefreshCheckBoxData;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
            }
        }

        private void RefreshData(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPageClient)
            {
                string query = "SELECT imie, nazwisko, pesel FROM KLIENT";

                var dataTable = DatabaseManager.ExecuteQuery(query);
                dataGridViewClient.DataSource = dataTable;
            }
        }

        private void RefreshCheckBoxData(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPageCar)
            {
                string query = "SELECT ";
                if (checkBoxCarLicensePlate.Checked) query += "numer_rejestracyjny, ";
                if (checkBoxCarModel.Checked) query += "model, ";
                if (checkBoxCarMarka.Checked) query += "marka, ";
                if (checkBoxCarYear.Checked) query += "rok_produkcji, ";
                query = query.TrimEnd(',', ' ') + " FROM SAMOCHOD";

                if (!checkBoxCarLicensePlate.Checked &&
                    !checkBoxCarModel.Checked &&
                    !checkBoxCarMarka.Checked &&
                    !checkBoxCarYear.Checked)
                {
                    query = "SELECT numer_rejestracyjny, model, marka, rok_produkcji FROM SAMOCHOD";
                }

                var dataTable = DatabaseManager.ExecuteQuery(query);
                dataGridViewCar.DataSource = dataTable;
            }
            if (tabControl1.SelectedTab == tabPageInsurance)
            {
                string query = "SELECT ";
                if (checkBoxInsuranceType.Checked) query += "rodzaj, ";
                if (checkBoxInsuranceDate.Checked) query += "data_waznosci, ";
                query = query.TrimEnd(',', ' ') + " FROM UBEZPIECZENIA";

                if (!checkBoxInsuranceType.Checked &&
                    !checkBoxInsuranceDate.Checked
                   )
                {
                    query = "SELECT rodzaj, data_waznosci FROM UBEZPIECZENIA";
                }

                var dataTable = DatabaseManager.ExecuteQuery(query);
                dataGridViewInsurance.DataSource = dataTable;
            }
            if (tabControl1.SelectedTab == tabPageBorrow)
            {
                string query = "SELECT ";
                if (checkBoxBorrowStartDate.Checked) query += "data_rozpoczecia, ";
                if (checkBoxBorrowDateEnd.Checked) query += "data_zakonczenia, ";
                query = query.TrimEnd(',', ' ') + " FROM WYPOZYCZENIE";

                if (!checkBoxBorrowStartDate.Checked &&
                    !checkBoxBorrowDateEnd.Checked
                   )
                {
                    query = "SELECT data_rozpoczecia, data_zakonczenia FROM WYPOZYCZENIE";
                }

                var dataTable = DatabaseManager.ExecuteQuery(query);
                dataGridViewBorrows.DataSource = dataTable;
            }
            if (tabControl1.SelectedTab == tabPageService)
            {
                string query = "SELECT ";
                if (checkBoxSerwisDate.Checked) query += "data_serwisu, ";
                if (checkBoxSerwisDescription.Checked) query += "opis, ";
                if (checkBoxSerwisPrice.Checked) query += "koszt, ";
                query = query.TrimEnd(',', ' ') + " FROM SERWIS";

                if (!checkBoxSerwisDate.Checked &&
                    !checkBoxSerwisDescription.Checked &&
                    !checkBoxSerwisPrice.Checked
                   )
                {
                    query = "SELECT data_serwisu, opis, koszt FROM SERWIS";
                }

                var dataTable = DatabaseManager.ExecuteQuery(query);
                dataGridViewSerwis.DataSource = dataTable;
            }
        }

        private void RefreshDataAddress(object sender, EventArgs e)
        {
            string query = "SELECT ulica, miejscowosc, kod_pocztowy from ADRES";
            var dataTable = DatabaseManager.ExecuteQuery(query);
            dataGridViewClient.DataSource = dataTable;
        }

        private void RefreshDataClientAndAddress(object sender, EventArgs e)
        {
            string query = "SELECT k.imie, k.nazwisko, k.pesel, a.ulica, a.miejscowosc, a.kod_pocztowy FROM klient k JOIN adres a ON k.id_klient = a.klient_id";
            var dataTable = DatabaseManager.ExecuteQuery(query);
            dataGridViewClient.DataSource = dataTable;
        }

        private void RefreshDataCar(object sender, EventArgs e)
        {
            string query = "SELECT numer_rejestracyjny, marka, model, rok_produkcji FROM SAMOCHOD";
            var dataTable = DatabaseManager.ExecuteQuery(query);
            dataGridViewCar.DataSource = dataTable;
        }

        private void RefreshDataRelationCarClient(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("wyswietl_klientow_i_samochody", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("kursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }
            }

            dataGridViewBorrows.DataSource = dt;
        }

        private void RefreshDataShowBorrows(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("wyswietl_wszystkie_wypozyczenia", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("kursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }
            }

            dataGridViewBorrows.DataSource = dt;
        }

        private void RefreshDataInsurance(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("wyswietl_wszystkie_samochody_i_ubezpieczenia", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("kursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }
            }

            dataGridViewInsurance.DataSource = dt;
        }

        private void RefreshDataSerwis(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("wyswietl_wszystkie_serwisy_i_samochody", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("kursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }
            }

            dataGridViewSerwis.DataSource = dt;
        }

        private void buttonClientAdd_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("dodaj_klienta", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_imie", OracleDbType.Varchar2).Value = textBoxClientName.Text;
                    cmd.Parameters.Add("p_nazwisko", OracleDbType.Varchar2).Value = textBoxClientSurname.Text;
                    cmd.Parameters.Add("p_pesel", OracleDbType.Varchar2).Value = textBoxClientPesel.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshData(null, null);
        }

        private void buttonClientShow_Click(object sender, EventArgs e)
        {
            RefreshData(null, null);
        }

        private void button19_Click(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {

        }

        private void buttonClientDelete_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("usun_klienta", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_imie", OracleDbType.Varchar2).Value = textBoxClientName.Text;
                    cmd.Parameters.Add("p_nazwisko", OracleDbType.Varchar2).Value = textBoxClientSurname.Text;
                    cmd.Parameters.Add("p_pesel", OracleDbType.Varchar2).Value = textBoxClientPesel.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        private void buttonClientSearch_Click(object sender, EventArgs e)
        {

            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("znajdz_klienta", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_imie", OracleDbType.Varchar2).Value = textBoxClientName.Text;
                    cmd.Parameters.Add("p_nazwisko", OracleDbType.Varchar2).Value = textBoxClientSurname.Text;
                    cmd.Parameters.Add("p_pesel", OracleDbType.Varchar2).Value = textBoxClientPesel.Text;
                    cmd.Parameters.Add("kursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }
            }

            dataGridViewClient.DataSource = dt;
        }

        private void label45_Click(object sender, EventArgs e)
        {

        }

        private void buttonAddressAdd_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("dodaj_adres", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_ulica", OracleDbType.Varchar2).Value = textBoxClientStreet.Text;
                    cmd.Parameters.Add("p_miejscowosc", OracleDbType.Varchar2).Value = textBoxClientCity.Text;
                    cmd.Parameters.Add("p_kod_pocztowy", OracleDbType.Varchar2).Value = textBoxClientPostCode.Text;
                    cmd.Parameters.Add("p_klient_id", OracleDbType.Int32).Value = DBNull.Value;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataAddress(null, null);
        }

        private void buttonAddressDelete_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("usun_adres", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_ulica", OracleDbType.Varchar2).Value = textBoxClientStreet.Text;
                    cmd.Parameters.Add("p_miejscowosc", OracleDbType.Varchar2).Value = textBoxClientCity.Text;
                    cmd.Parameters.Add("p_kod_pocztowy", OracleDbType.Varchar2).Value = textBoxClientPostCode.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataAddress(null, null);
        }

        private void buttonAddressSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("znajdz_adres", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_ulica", OracleDbType.Varchar2).Value = textBoxClientStreet.Text;
                    cmd.Parameters.Add("p_miejscowosc", OracleDbType.Varchar2).Value = textBoxClientCity.Text;
                    cmd.Parameters.Add("p_kod_pocztowy", OracleDbType.Varchar2).Value = textBoxClientPostCode.Text;
                    cmd.Parameters.Add("kursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }
            }

            dataGridViewClient.DataSource = dt;
        }

        private void buttonAddressShow_Click(object sender, EventArgs e)
        {
            RefreshDataAddress(null, null);
        }

        private void buttonClientToAddress_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("przypisz_klienta_do_adresu", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_imie", OracleDbType.Varchar2).Value = textBoxClientName.Text;
                    cmd.Parameters.Add("p_nazwisko", OracleDbType.Varchar2).Value = textBoxClientSurname.Text;
                    cmd.Parameters.Add("p_pesel", OracleDbType.Varchar2).Value = textBoxClientPesel.Text;
                    cmd.Parameters.Add("p_ulica", OracleDbType.Varchar2).Value = textBoxClientStreet.Text;
                    cmd.Parameters.Add("p_miejscowosc", OracleDbType.Varchar2).Value = textBoxClientCity.Text;
                    cmd.Parameters.Add("p_kod_pocztowy", OracleDbType.Varchar2).Value = textBoxClientPostCode.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataClientAndAddress(null, null);
        }

        private void buttonClientToAddressShow_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("wyswietl_klientow_z_adresami", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("kursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }
            }

            dataGridViewClient.DataSource = dt;
        }

        private void buttonClientToAddressDelete_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("usun_polaczenie_klient_adres", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_ulica", OracleDbType.Varchar2).Value = textBoxClientStreet.Text;
                    cmd.Parameters.Add("p_miejscowosc", OracleDbType.Varchar2).Value = textBoxClientCity.Text;
                    cmd.Parameters.Add("p_kod_pocztowy", OracleDbType.Varchar2).Value = textBoxClientPostCode.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataClientAndAddress(null, null);
        }

        private void buttonClientToAddressSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("wyszukaj_klienta_z_adresem", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_imie", OracleDbType.Varchar2).Value = textBoxClientName.Text;
                    cmd.Parameters.Add("p_nazwisko", OracleDbType.Varchar2).Value = textBoxClientSurname.Text;
                    cmd.Parameters.Add("p_pesel", OracleDbType.Varchar2).Value = textBoxClientPesel.Text;
                    cmd.Parameters.Add("p_ulica", OracleDbType.Varchar2).Value = textBoxClientStreet.Text;
                    cmd.Parameters.Add("p_miejscowosc", OracleDbType.Varchar2).Value = textBoxClientCity.Text;
                    cmd.Parameters.Add("p_kod_pocztowy", OracleDbType.Varchar2).Value = textBoxClientPostCode.Text;
                    cmd.Parameters.Add("kursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }
            }

            dataGridViewClient.DataSource = dt;
        }

        private void buttonCarAdd_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("dodaj_samochod", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxCarLicensePlate.Text;
                    cmd.Parameters.Add("p_marka", OracleDbType.Varchar2).Value = textBoxCarCompany.Text;
                    cmd.Parameters.Add("p_model", OracleDbType.Varchar2).Value = textBoxCarModel.Text;
                    cmd.Parameters.Add("p_rok_produkcji", OracleDbType.Int32).Value = int.Parse(textBoxCarYear.Text);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataCar(null, null);
        }

        private void buttonCarDelete_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("usun_samochod", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxCarLicensePlate.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataCar(null, null);
        }

        private void buttonCarShow_Click(object sender, EventArgs e)
        {
            RefreshDataCar(null, null);
        }

        private void buttonCarSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("wyszukaj_samochod", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxCarLicensePlate.Text;
                    cmd.Parameters.Add("kursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }
            }

            dataGridViewCar.DataSource = dt;
        }

        private void buttonCarAttachClient_Click(object sender, EventArgs e)
        {
        }

        private void buttonCarShowClientAndCar_Click(object sender, EventArgs e)
        {

        }

        private void buttonCarSearchClientAndCar_Click(object sender, EventArgs e)
        {

        }

        private void buttonBorrowClientAddCar_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("przypisz_samochod_do_klienta", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxBorrowCarLicensePlate.Text;
                    cmd.Parameters.Add("p_pesel", OracleDbType.Varchar2).Value = textBoxBorrowClientPesel.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataRelationCarClient(null, null);
        }

        private void buttonBorrowClientDeleteCar_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("usun_polaczenie_samochod_klient", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_pesel", OracleDbType.Varchar2).Value = textBoxBorrowClientPesel.Text;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxBorrowCarLicensePlate.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataRelationCarClient(null, null);
        }

        private void buttonBorrowClientSearchCar_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("wyszukaj_samochod_i_klienta", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxBorrowCarLicensePlate.Text;
                    cmd.Parameters.Add("p_pesel", OracleDbType.Varchar2).Value = textBoxBorrowClientPesel.Text;
                    cmd.Parameters.Add("kursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }
            }

            dataGridViewBorrows.DataSource = dt;
        }

        private void buttonBorrowClientShowCar_Click(object sender, EventArgs e)
        {
            RefreshDataRelationCarClient(null, null);
        }

        private void buttonBorrowAdd_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("dodaj_wypozyczenie", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_pesel", OracleDbType.Varchar2).Value = textBoxBorrowClientPesel.Text;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxBorrowCarLicensePlate.Text;
                    cmd.Parameters.Add("p_data_rozpoczecia", OracleDbType.Date).Value = DateTime.Parse(textBoxBorrowStartDate.Text);
                    cmd.Parameters.Add("p_data_zakonczenia", OracleDbType.Date).Value = DateTime.Parse(textBoxBorrowEndDate.Text);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataShowBorrows(null, null);
        }

        private void buttonBorrowDelete_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("usun_wypozyczenie", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_pesel", OracleDbType.Varchar2).Value = textBoxBorrowClientPesel.Text;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxBorrowCarLicensePlate.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataShowBorrows(null, null);
        }

        private void buttonBorrowSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("wyswietl_wypozyczenie", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_pesel", OracleDbType.Varchar2).Value = textBoxBorrowClientPesel.Text;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxBorrowCarLicensePlate.Text;
                    cmd.Parameters.Add("kursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }
            }

            dataGridViewBorrows.DataSource = dt;
        }

        private void buttonBorrowShow_Click(object sender, EventArgs e)
        {
            RefreshDataShowBorrows(null, null);
        }

        private void buttonInsuranceAdd_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("dodaj_ubezpieczenie", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxInsuranceCarPlates.Text;
                    cmd.Parameters.Add("p_rodzaj", OracleDbType.Varchar2).Value = textBoxInsuranceType.Text;
                    cmd.Parameters.Add("p_data_waznosci", OracleDbType.Date).Value = DateTime.Parse(textBoxInsuranceDate.Text);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataInsurance(null, null);
        }

        private void buttonInsuranceDelete_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("usun_ubezpieczenie", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxInsuranceCarPlates.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataInsurance(null, null);
        }

        private void buttonInsuranceSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("wyszukaj_auto_i_ubezpieczenie", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxInsuranceCarPlates.Text;
                    cmd.Parameters.Add("kursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }
            }

            dataGridViewInsurance.DataSource = dt;
        }

        private void buttonInsuranceShow_Click(object sender, EventArgs e)
        {
            RefreshDataInsurance(null, null);
        }

        private void buttonSerwisAdd_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("dodaj_do_serwisu", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxSerwisCarLicensePlates.Text;
                    cmd.Parameters.Add("p_opis", OracleDbType.Varchar2).Value = textBoxSerwisDescription.Text;
                    cmd.Parameters.Add("p_data_serwisu", OracleDbType.Date).Value = DateTime.Parse(textBoxSerwisDate.Text);
                    cmd.Parameters.Add("p_koszt", OracleDbType.Decimal).Value = decimal.Parse(textBoxSerwisPrice.Text);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataSerwis(null, null);
        }

        private void buttonSerwisDelete_Click(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("usun_serwis_po_numerze_rejestracyjnym", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxSerwisCarLicensePlates.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            RefreshDataSerwis(null, null);
        }

        private void buttonSerwisSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("wyswietl_serwis_po_numerze_rejestracyjnym", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_numer_rejestracyjny", OracleDbType.Varchar2).Value = textBoxSerwisCarLicensePlates.Text;
                    cmd.Parameters.Add("kursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }
            }

            dataGridViewSerwis.DataSource = dt;
        }

        private void buttonSerwisShow_Click(object sender, EventArgs e)
        {
            RefreshDataSerwis(null, null);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCarAutoRefresh.Checked)
            {
                refreshTimer.Start();
            }
            else { refreshTimer.Stop(); }
        }

        private void buttonCarCheckBoxCheckerShow_Click(object sender, EventArgs e)
        {
            RefreshCheckBoxData(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RefreshCheckBoxData(null, null);
        }

        private void checkBoxInsuranceAutoRefhresh_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxInsuranceAutoRefhresh.Checked)
            {
                refreshTimer.Start();
            }
            else { refreshTimer.Stop(); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RefreshCheckBoxData(null, null);
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                refreshTimer.Start();
            }
            else { refreshTimer.Stop(); }
        }

        private void checkBoxSerwisAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSerwisAutoRefresh.Checked)
            {
                refreshTimer.Start();
            }
            else { refreshTimer.Stop(); }
        }

        private void buttonSerwisShowCheckBox_Click(object sender, EventArgs e)
        {
            RefreshCheckBoxData(null, null);
        }
    }
}

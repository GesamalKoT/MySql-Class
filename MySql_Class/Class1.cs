using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;

namespace MySql_Class
{
    public static class Start
    {
        //public MySqlConnection globalConnection = new MySqlConnection();

        static public MySqlConnection BD_Check(string[] MySql_Settings)
        {
            string MySql_user_Name = MySql_Settings[0];
            string MySql_password = MySql_Settings[1];

            string conString = "Data Source = localhost; Port = 3306; User Id = '" + MySql_user_Name + "';" +
                " Password = '" + MySql_password + "'; Database = Dormitory_DB; CharSet=utf8;";

            MySqlConnection newConnection = new MySqlConnection(conString); // Dormitory - общежитие
            MySqlCommand Query = new MySqlCommand();
            Query.Connection = newConnection;
            try
            {
                newConnection.Open();
                /*MySqlCommand cod = new MySqlCommand();
                cod.Connection = newConnection;
                cod.CommandText = "SET NAMES utf8;";
                cod.ExecuteNonQuery();*/
            }
            catch
            {
                BD_Creator(ref newConnection, conString, MySql_Settings);
            }
            return newConnection;
        }

        static public void BD_Creator(ref MySqlConnection newConnection, string conString, string[] MySql_Settings)
        {
            newConnection.Close();

            string MySql_user_Name = MySql_Settings[0];
            string MySql_password = MySql_Settings[1];
            newConnection = new MySqlConnection("Data Source = localhost; Port = 3306;  User Id = '" + MySql_user_Name + "'; Password = '" + MySql_password + "'; CharSet=utf8;"); // строка подключения к серверу

            MySqlCommand Command = new MySqlCommand();
            Command.Connection = newConnection;
            Command.CommandText = "CREATE DATABASE IF NOT EXISTS Dormitory_DB;"; // Команда на создание БД

            newConnection.Open();
            Command.ExecuteNonQuery();
            newConnection.Close();

            newConnection = new MySqlConnection(conString);
            newConnection.Open();

            /*MySqlCommand cod = new MySqlCommand();
            cod.Connection = newConnection;
            cod.CommandText = "SET NAMES utf8;";
            cod.ExecuteNonQuery();*/

            table_Creator(newConnection, MySql_Settings[2]);
        }

        static public void table_Creator(MySqlConnection newConnection, string hous)
        {
            MySqlCommand create_StudentsTable = new MySqlCommand();
            create_StudentsTable.Connection = newConnection;
            create_StudentsTable.CommandText = "CREATE TABLE IF NOT EXISTS Students(Surname CHAR(50) NOT NULL, Name CHAR(50) NOT NULL, Patronymic CHAR(50), Student_ID CHAR(50) NOT NULL PRIMARY KEY, Gender CHAR(10) NOT NULL, Faculty CHAR(5) NOT NULL, Course INT NOT NULL, Room INT, Date DATETIME, Phone CHAR(50), Decree CHAR(80) NOT NULL, DecreeDate DATETIME NOT NULL, Form CHAR(10) NOT NULL, StayLimit DateTime, Citizenship CHAR(40), evicted BOOL, EvictedTill DATETIME);";
            //create_StudentsTable.CommandText = "CREATE TABLE IF NOT EXISTS Students(Ord CHAR(50) NOT NULL);";
            create_StudentsTable.ExecuteNonQuery();

            MySqlCommand create_RoomsTable = new MySqlCommand();
            create_StudentsTable.Connection = newConnection;
            create_StudentsTable.CommandText = "CREATE TABLE IF NOT EXISTS Rooms(Housing INT NOT NULL, Number INT NOT NULL PRIMARY KEY, Places INT NOT NULL, FPlaces INT NOT NULL, Gender CHAR(2));";
            create_StudentsTable.ExecuteNonQuery();

            MySqlCommand create_UsersTable = new MySqlCommand();
            create_UsersTable.Connection = newConnection;
            create_UsersTable.CommandText = "CREATE TABLE IF NOT EXISTS Users(Name CHAR(50) NOT NULL PRIMARY KEY, Password CHAR(50) NOT NULL);";
            create_UsersTable.ExecuteNonQuery();

            MySqlCommand create_ArchiveTable = new MySqlCommand();
            create_ArchiveTable.Connection = newConnection;
            create_ArchiveTable.CommandText = "CREATE TABLE IF NOT EXISTS Archive(Surname CHAR(50) NOT NULL, Name CHAR(50) NOT NULL, Patronymic CHAR(50), Student_ID CHAR(50) NOT NULL PRIMARY KEY, Gender CHAR(10) NOT NULL, Decree CHAR(80), Date DATETIME);";
            create_ArchiveTable.ExecuteNonQuery();

            MySqlCommand create_CitizenshipTable = new MySqlCommand();
            create_CitizenshipTable.Connection = newConnection;
            create_CitizenshipTable.CommandText = "CREATE TABLE IF NOT EXISTS Citizenship(Name CHAR(20) NOT NULL PRIMARY KEY, Fullname CHAR(30) NOT NULL);";
            create_CitizenshipTable.ExecuteNonQuery();
            MySqlCommand add_Citizenship = new MySqlCommand();
            add_Citizenship.Connection = newConnection;
            add_Citizenship.CommandText = "INSERT INTO Citizenship VALUES ('РФ', 'Российская Федерация'), ('Украина', 'Украина'), ('Белоруссия', 'Республика Белорусь'), ('Казахстан', 'Республика Казахстан'), ('Узбекистан','Республика Узбекистан');";
            add_Citizenship.ExecuteNonQuery();

            MySqlCommand create_otherLivingTable = new MySqlCommand();
            create_otherLivingTable.Connection = newConnection;
            create_otherLivingTable.CommandText = "CREATE TABLE IF NOT EXISTS OtherLiving(ID CHAR(50) PRIMARY KEY, Surname CHAR(50) NOT NULL, Name CHAR(50) NOT NULL, Patronymic CHAR(50), Gender CHAR(10), Room INT, Date DateTime, Decree CHAR(80) NOT NULL, DecreeDate DATETIME, StayLimit DateTime, Citizenship CHAR(40), Info CHAR(100), Phone CHAR(50), evicted BOOL,EvictedTill DATETIME);";
            create_otherLivingTable.ExecuteNonQuery();

            MySqlCommand create_otherLivingIdTable = new MySqlCommand();
            create_otherLivingTable.Connection = newConnection;
            create_otherLivingTable.CommandText = "CREATE TABLE IF NOT EXISTS OtherLivingID(ID CHAR(50) NOT NULL PRIMARY KEY);";
            create_otherLivingTable.ExecuteNonQuery();

            MySqlCommand add_firstOtherLiving = new MySqlCommand();
            add_firstOtherLiving.Connection = newConnection;
            add_firstOtherLiving.CommandText = "INSERT INTO OtherLivingID Values('5" + hous + "0000');";
            add_firstOtherLiving.ExecuteNonQuery();

            MySqlCommand create_SettingsTable = new MySqlCommand();
            create_SettingsTable.Connection = newConnection;
            create_SettingsTable.CommandText = "CREATE TABLE IF NOT EXISTS Settings(ID INT,FIO CHAR(80), Housing CHAR(10), OutTime CHAR(50), SpringPay DATETIME, AutumnPay DATETIME, CheckPeriod char(50), LastCheckDate DATETIME);";
            create_SettingsTable.ExecuteNonQuery();

            int c = 1;

            MySqlCommand insertSettingsTable = new MySqlCommand();
            insertSettingsTable.Connection = newConnection;
            insertSettingsTable.CommandText = "INSERT INTO Settings(ID) VALUES(1);";
            insertSettingsTable.ExecuteNonQuery();

            add_Admin(newConnection);
        }

        static public void add_Admin(MySqlConnection newConnection)
        {
            string password = Security("1811");
            MySqlCommand add_Admin = new MySqlCommand();
            add_Admin.Connection = newConnection;
            add_Admin.CommandText = "INSERT INTO USERS VALUES('admin' , '" + password + "');";
            add_Admin.ExecuteNonQuery();
        }

        static public string Security(string Password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(Password));
            string resault = BitConverter.ToString(checkSum).Replace("-", String.Empty);
            return resault;
        }

        static public int enter(string user_Name, string user_Password, MySqlConnection globalConnection)
        {
            MySqlCommand search_User = new MySqlCommand();
            search_User.Connection = globalConnection;
            search_User.CommandText = "SELECT * FROM Users WHERE Name = '" + user_Name + "'";
            search_User.ExecuteNonQuery();
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(search_User);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            var myData = dt.Select();
            string newPassword = Security(user_Password);
            if (myData.Length == 0)
                return 0;
            else
                if (myData[0].ItemArray[1].ToString() == newPassword)
                    if (user_Name == "admin")
                        return 1;
                    else
                        return 2;
                else
                    return -1;
        }

    }

    public static class Administrator
    {
        static public int add_NewUser(string User, string Password, MySqlConnection globalConnection)
        {
            MySqlCommand search_User = new MySqlCommand();
            search_User.Connection = globalConnection;
            search_User.CommandText = "SELECT * FROM Users WHERE Name = '" + User + "';";
            search_User.ExecuteNonQuery();
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(search_User);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            var myData = dt.Select();
            if (myData.Length == 0)
            {
                MySqlCommand add_User = new MySqlCommand();
                add_User.Connection = globalConnection;
                string newPassword = Security(Password);
                add_User.CommandText = "INSERT INTO Users VALUES('" + User + "' , '" + newPassword + "');";
                add_User.ExecuteNonQuery();
                return 1;
            }
            else
                return 0;
        }

        static public void delete_User(string User, MySqlConnection globalConnection)
        {
            MySqlCommand del_user = new MySqlCommand();
            del_user.Connection = globalConnection;
            del_user.CommandText = "DELETE FROM Users WHERE Name = '" + User + "';";
            del_user.ExecuteNonQuery();
        }

        static public int add_NewRoom(MySqlConnection globalConnection, string Housing, string Number, string Places)
        {
            int iHousing = Convert.ToInt32(Housing);
            int iNumber = Convert.ToInt32(Number);
            int iPlaces = Convert.ToInt32(Places);
            string Gender = "";

            MySqlCommand search_Room = new MySqlCommand();
            search_Room.Connection = globalConnection;
            search_Room.CommandText = "SELECT * FROM Rooms WHERE Number = '" + iNumber + "';";
            search_Room.ExecuteNonQuery();
            MySqlDataAdapter adapt = new MySqlDataAdapter(search_Room);
            DataTable dt = new DataTable();
            adapt.Fill(dt);
            var myData = dt.Select();

            if (myData.Length == 0)
            {
                MySqlCommand add_NewRoom = new MySqlCommand();
                add_NewRoom.Connection = globalConnection;
                add_NewRoom.CommandText = "INSERT INTO Rooms VALUES('" + iHousing + "', '" + iNumber + "', '" + iPlaces + "', '" + iPlaces + "', '" + Gender + "');";
                add_NewRoom.ExecuteNonQuery();
                return 1;
            }
            else
                return 0;
        }

        static public void delete_Room(MySqlConnection globalConnection, int Number)
        {
            MySqlCommand deleteRoom = new MySqlCommand();
            deleteRoom.Connection = globalConnection;
            deleteRoom.CommandText = "DELETE FROM Rooms WHERE Number = '" + Number + "';";
            deleteRoom.ExecuteNonQuery();
        }

        static public string Security(string Password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(Password));
            string resault = BitConverter.ToString(checkSum).Replace("-", String.Empty);
            return resault;
        }
    }

    public static class Common_Functions
    {


        public static void update_RoomInfo(MySqlConnection globalConnection, string number, string gender)
        {
            int iNumber = Convert.ToInt32(number);

            MySqlCommand srcRoom = new MySqlCommand();
            srcRoom.Connection = globalConnection;
            srcRoom.CommandText = "SELECT * FROM Rooms WHERE Number = '" + iNumber + "';";
            srcRoom.ExecuteNonQuery();

            MySqlDataAdapter da = new MySqlDataAdapter(srcRoom);
            DataTable dt = new DataTable();
            da.Fill(dt);
            var Mydata = dt.Select();
            if (Mydata[0].ItemArray[4].ToString() == "")
            {
                MySqlCommand freeRoom = new MySqlCommand();
                freeRoom.Connection = globalConnection;
                freeRoom.CommandText = "UPDATE Rooms SET Gender = '" + gender + "', Fplaces = Fplaces - 1 WHERE Number = '" + iNumber + "';";
                freeRoom.ExecuteNonQuery();
            }
            else
            {
                MySqlCommand not_FreeRoom = new MySqlCommand();
                not_FreeRoom.Connection = globalConnection;
                not_FreeRoom.CommandText = "UPDATE Rooms SET Fplaces = Fplaces - 1 Where Number = '" + iNumber + "';";
                not_FreeRoom.ExecuteNonQuery();
            }
        }

        public static void update_StudentRoomInfo(MySqlConnection globalconnection, string room_Number, string studentID)
        {
            int iNumber = Convert.ToInt32(room_Number);

            MySqlCommand update_SRI = new MySqlCommand();
            update_SRI.Connection = globalconnection;
            update_SRI.CommandText = "UPDATE Students SET Room = '" + iNumber + "' WHERE Student_ID = '" + studentID + "';";
            update_SRI.ExecuteNonQuery();

        }

        public static void update_OtherLivingRoomInfo(MySqlConnection globalConnection, string room_Number, string ID)
        {
            int iNumber = Convert.ToInt32(room_Number);

            MySqlCommand update_OLRI = new MySqlCommand();
            update_OLRI.Connection = globalConnection;
            update_OLRI.CommandText = "UPDATE OtherLiving SET Room = '" + iNumber + "' WHERE ID = '" + ID + "';";
            update_OLRI.ExecuteNonQuery();
        }

        static public void delete_FromRoom(MySqlConnection globalConnection, string Number)
        {

            int iNumber = Convert.ToInt32(Number);

            /*MySqlCommand delete = new MySqlCommand();
            delete.Connection = globalConnection;
            delete.CommandText = "UPDATE Rooms SET Fplaces = Fplaces + 1 WHERE Number = '"+Number+"';";
            delete.ExecuteNonQuery();*/

            MySqlCommand select = new MySqlCommand();
            select.Connection = globalConnection;
            select.CommandText = "SELECT * FROM Rooms WHERE Number = '" + iNumber + "';";
            select.ExecuteNonQuery();

            MySqlDataAdapter da = new MySqlDataAdapter(select);
            DataTable dt1 = new DataTable();
            da.Fill(dt1);
            var myData1 = dt1.Select();

            if (Convert.ToInt32(myData1[0].ItemArray[3]) >= Convert.ToInt32(myData1[0].ItemArray[2]) - 1)
            {
                select.CommandText = "UPDATE Rooms SET Fplaces = Places WHERE Number = '" + Number + "';";
            }
            else
            {
                select.CommandText = "UPDATE Rooms SET Fplaces = Fplaces + 1 WHERE Number = '" + Number + "';";
            }
            select.ExecuteNonQuery();

            MySqlCommand set_NullSex = new MySqlCommand();
            set_NullSex.Connection = globalConnection;
            set_NullSex.CommandText = "SELECT * FROM Rooms WHERE Number = '" + Number + "';";
            set_NullSex.ExecuteNonQuery();

            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(set_NullSex);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            var MyData = dt.Select();

            if (MyData[0].ItemArray[2].Equals(MyData[0].ItemArray[3]))
            {
                set_NullSex.CommandText = "UPDATE Rooms SET Gender = '' WHERE NUMBER = '" + Number + "';";
                set_NullSex.ExecuteNonQuery();
            }

            /*MySqlCommand update_SexInformation = new MySqlCommand();
            update_SexInformation.Connection = globalConnection;
            update_SexInformation.CommandText = "SELECT Sex, IF(Fplaces == Places, NULL, Fplaces) AS Fplaces FROM Rooms;";
            update_SexInformation.ExecuteNonQuery();*/
        }

        public static ObservableCollection<RoomItem> select_RoomForFamily(MySqlConnection globalConnection)
        {
            ObservableCollection<RoomItem> resault = new ObservableCollection<RoomItem>();

            MySqlCommand select_Room = new MySqlCommand();
            select_Room.Connection = globalConnection;
            select_Room.CommandText = "SELECT * FROM Rooms WHERE Gender = '' AND Places = 2 AND Fplaces = 2;";
            select_Room.ExecuteNonQuery();

            MySqlDataAdapter da = new MySqlDataAdapter(select_Room);
            DataTable dt = new DataTable();
            da.Fill(dt);
            var myData = dt.Select();

            for (int i = 0; i < myData.Length; i++)
            {
                resault.Add(new RoomItem { inc = i + 1, room_Numb = myData[i].ItemArray[1].ToString(), 
                                           places_Con = myData[i].ItemArray[2].ToString(), Fplaces_Con = myData[i].ItemArray[3].ToString(),
                                           type = "пустая" });
            }

            return resault;
        }

        public static DataTable room_Selection(MySqlConnection globalConnection, string Sex)
        {
            MySqlCommand select_Room = new MySqlCommand();
            select_Room.Connection = globalConnection;
            select_Room.CommandText = "SELECT * FROM Rooms WHERE ((Gender = '" + Sex + " OR Gender = '') AND Fplaces > 0);";
            select_Room.ExecuteNonQuery();

            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(select_Room);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            return dt;
        }

        public static DataTable serch_RoomByNumber(MySqlConnection globalConnection, string Number)
        {
            int iNumber = Convert.ToInt32(Number);
            MySqlCommand serch_Room = new MySqlCommand();
            serch_Room.Connection = globalConnection;
            serch_Room.CommandText = "SELECT * FROM Rooms WHERE Number = '" + iNumber + "';";
            serch_Room.ExecuteNonQuery();

            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(serch_Room);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            return dt;
        }

        public static void fake_AddStudent(MySqlConnection globalConnection, string studentID)
        {
            string dat = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");

            MySqlCommand fakeAddition = new MySqlCommand();
            fakeAddition.Connection = globalConnection;
            string student_Surname = "FAKE";
            string student_Name = "FAKE";
            string student_Patronymic = "FAKE";
            string gender = "G";
            string faculty = "F";
            int iCourse = 1;
            int iRoom = 1;
            string Phone = "P";
            string Decree = "D";
            string Form = "F";
            string Citizenship = "C";

            fakeAddition.CommandText = "INSERT INTO Students VALUES ('" + student_Surname + "', '" + student_Name + "', '" + student_Patronymic + "', '" + studentID + "', '" + gender + "','" + faculty + "', '" + iCourse + "', '" + iRoom + "', '" + dat + "', '" + Phone + "', '" + Decree + "', '" + dat + "', '" + Form + "', '" + dat + "', '" + Citizenship + "', 0, null);";
            fakeAddition.ExecuteNonQuery();
        }



        public static void refresh_StudentData(MySqlConnection globalConnection, string Name, string Surname, string Patronimic, string StudentID, string Gender, string Faculty, string Course, string Phone, string Decree, string Form, string Citizenship, DateTime date, DateTime DecreeDate, DateTime StayLimit)
        {
            /*DateTime date = DateTime.Now;
            DateTime DecreeDate = DateTime.Now;
            DateTime StayLimit = DateTime.Now;*/
            string PayDate = date.ToString("yyyy-MM-dd H:mm:ss");
            /*if (date.Date == DateTime.Now.Date.AddDays(-1))
                PayDate = "";*/
            string DecrDate = DecreeDate.ToString("yyyy-MM-dd H:mm:ss");
            string StayLim = StayLimit.ToString("yyyy-MM-dd H:mm:ss");

            MySqlCommand updateStudentInfo = new MySqlCommand();
            updateStudentInfo.Connection = globalConnection;
            updateStudentInfo.CommandText = "UPDATE Students SET Name = '" + Name + "', Surname = '" + Surname + "', Patronymic = '" + Patronimic + "', Student_ID = '" + StudentID + "', Gender = '" + Gender + "', Faculty = '" + Faculty + "',  Course = '" + Course + "', Date = '" + PayDate + "', Phone = '" + Phone + "', Decree = '" + Decree + "', DecreeDate = '" + DecrDate + "',  Form = '" + Form + "', StayLimit = '" + StayLim + "', Citizenship = (Select Fullname FROM CITIZENSHIP WHERE Name = '" + Citizenship + "'), evicted = 0 WHERE Name = 'FAKE' ;";
            updateStudentInfo.ExecuteNonQuery();
        }

        public static void refresh_OtherLivingData(MySqlConnection globalConnection, string otherID, string otherName, string otherSurname, string otherPatronimic, string otherGender, DateTime date, string decree, DateTime decreeDate, DateTime stayLimit, string citizenship, string info, string phone)
        {
            string PayDate = date.ToString("yyyy-MM-dd H:mm:ss");
            /*if (date.Date == DateTime.Now.Date.AddDays(-1))
                PayDate = "";*/
            string DecrDate = decreeDate.ToString("yyyy-MM-dd H:mm:ss");
            string StayLim = stayLimit.ToString("yyyy-MM-dd H:mm:ss");

            MySqlCommand updateOtherlivingInfo = new MySqlCommand();
            updateOtherlivingInfo.Connection = globalConnection;
            updateOtherlivingInfo.CommandText = "UPDATE OtherLiving SET ID = '" + otherID + "', Name = '" + otherName + "', Surname = '" + otherSurname + "', Patronymic = '" + otherPatronimic + "', Gender = '" + otherGender + "', Date = '" + PayDate + "', Decree = '" + decree + "', DecreeDate = '" + DecrDate + "', StayLimit = '" + StayLim + "', Citizenship = (Select Fullname FROM CITIZENSHIP WHERE Name = '" + citizenship + "'), Info = '" + info + "', phone = '" + phone + "', evicted = 0 WHERE Name = 'FAKE';";
            updateOtherlivingInfo.ExecuteNonQuery();
        }

        public static void update_StudentData(MySqlConnection globalConnection, string defaultID, string Name, string Surname, string Patronimic, string StudentID, string Faculty, string Course, string Phone, string Decree, string Form, string Citizenship, DateTime date, DateTime DecreeDate, DateTime StayLimit)
        {
            string PayDate = date.ToString("yyyy-MM-dd H:mm:ss");
            string DecrDate = DecreeDate.ToString("yyyy-MM-dd H:mm:ss");
            string StayLim = StayLimit.ToString("yyyy-MM-dd H:mm:ss");

            MySqlCommand updateStudentInfo = new MySqlCommand();
            updateStudentInfo.Connection = globalConnection;
            updateStudentInfo.CommandText = "UPDATE Students SET Name = '" + Name + "', Surname = '" + Surname + "', Patronymic = '" + Patronimic + "', Student_ID = '" + StudentID + "', Faculty = '" + Faculty + "',  Course = '" + Course + "', Date = '" + PayDate + "', Phone = '" + Phone + "', Decree = '" + Decree + "', DecreeDate = '" + DecrDate + "',  Form = '" + Form + "', StayLimit = '" + StayLim + "', Citizenship = (Select Fullname FROM CITIZENSHIP WHERE Name = '" + Citizenship + "') WHERE Student_ID = '" + defaultID + "' ;";
            updateStudentInfo.ExecuteNonQuery();
            
        }

        public static void update_OtherLivingData(MySqlConnection globalConnection, string otherID, string otherName, string otherSurname, string otherPatronimic, DateTime date, string decree, DateTime decreeDate, DateTime stayLimit, string citizenship, string info, string phone)
        {
            string PayDate = date.ToString("yyyy-MM-dd H:mm:ss");
            /*if (date.Date == DateTime.Now.Date.AddDays(-1))
                PayDate = "";*/
            string DecrDate = decreeDate.ToString("yyyy-MM-dd H:mm:ss");
            string StayLim = stayLimit.ToString("yyyy-MM-dd H:mm:ss");

            MySqlCommand updateOtherlivingInfo = new MySqlCommand();
            updateOtherlivingInfo.Connection = globalConnection;
            updateOtherlivingInfo.CommandText = "UPDATE OtherLiving SET ID = '" + otherID + "', Name = '" + otherName + "', Surname = '" + otherSurname + "', Patronymic = '" + otherPatronimic + "', Date = '" + PayDate + "', Decree = '" + decree + "', DecreeDate = '" + DecrDate + "', StayLimit = '" + StayLim + "', Citizenship = (Select Fullname FROM CITIZENSHIP WHERE Name = '" + citizenship + "'), Info = '" + info + "', phone = '" + phone + "' WHERE ID = '" + otherID + "';";
            updateOtherlivingInfo.ExecuteNonQuery();
        }

        static public string getNewID(MySqlConnection globalConnection, int check)
        {
            MySqlCommand getID = new MySqlCommand();
            getID.Connection = globalConnection;
            getID.CommandText = "SELECT MAX(ID) FROM OtherLivingID;";
            getID.ExecuteNonQuery();

            MySqlDataAdapter da = new MySqlDataAdapter(getID);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var myData = dt.Select();
            string id = (Convert.ToInt32(myData[0].ItemArray[0]) + 1).ToString();

            if (check == 0)
            {
                MySqlCommand addNewId = new MySqlCommand();
                addNewId.Connection = globalConnection;
                addNewId.CommandText = "INSERT INTO OtherLivingID VALUES('" + id + "');";
                addNewId.ExecuteNonQuery();
            }
            else
            {
                id = (myData[0].ItemArray[0]).ToString();
            }
            return id;
        }

        static public void fake_addOtherLiving(MySqlConnection globalConnection, string OtherLivingID)
        {
            MySqlCommand addFake = new MySqlCommand();
            addFake.Connection = globalConnection;

            string student_Surname = "FAKE";
            string student_Name = "FAKE";
            string student_Patronymic = "FAKE";
            string gender = "G";
            int iRoom = 1;
            string Phone = "P";
            string Decree = "D";
            string Citizenship = "C";
            string info = "i";
            string dat = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");

            addFake.CommandText = "INSERT INTO OtherLiving VALUES ('" + OtherLivingID + "', '" + student_Surname + "', '" + student_Name + "', '" + student_Patronymic + "', '" + gender + "', '" + iRoom + "', '" + dat + "','" + Decree + "','" + dat + "','" + dat + "', '" + Citizenship + "', '" + info + "','" + Phone + "', 0, null);";
            addFake.ExecuteNonQuery();
        }

        public static void evictionST(MySqlConnection globalConnection, string ID, string decree, DateTime date)
        {
            string dat = date.ToString("yyyy-MM-dd");

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Students WHERE Student_ID = '" + ID + "';", globalConnection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            var myData = dt.Select();

            MySqlCommand insertSTarc = new MySqlCommand();
            insertSTarc.Connection = globalConnection;
            insertSTarc.CommandText = "INSERT INTO Archive VALUES('" + myData[0].ItemArray[0].ToString() + "', '" + myData[0].ItemArray[1].ToString() + "', '" + myData[0].ItemArray[2].ToString() + "', '" + myData[0].ItemArray[3].ToString() + "', '" + myData[0].ItemArray[4].ToString() + "', '" + decree + "', '" + dat + "');";
            insertSTarc.ExecuteNonQuery();

            delete_FromRoom(globalConnection, myData[0].ItemArray[7].ToString());

            MySqlCommand delete = new MySqlCommand("DELETE FROM Students WHERE Student_ID = '" + ID + "';", globalConnection);
            delete.ExecuteNonQuery();
        }

        public static void evictionSTE(MySqlConnection globalConnection, string ID, string decree, DateTime date)
        {
            string dat = date.ToString("yyyy-MM-dd");

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Students WHERE Student_ID = '" + ID + "';", globalConnection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            var myData = dt.Select();

            MySqlCommand insertSTarc = new MySqlCommand();
            insertSTarc.Connection = globalConnection;
            insertSTarc.CommandText = "INSERT INTO Archive VALUES('" + myData[0].ItemArray[0].ToString() + "', '" + myData[0].ItemArray[1].ToString() + "', '" + myData[0].ItemArray[2].ToString() + "', '" + myData[0].ItemArray[3].ToString() + "', '" + myData[0].ItemArray[4].ToString() + "', '" + decree + "', '" + dat + "');";
            insertSTarc.ExecuteNonQuery();

            MySqlCommand delete = new MySqlCommand("DELETE FROM Students WHERE Student_ID = '" + ID + "';", globalConnection);
            delete.ExecuteNonQuery();
        }

        public static void evictionOL(MySqlConnection globalConnection, string ID, string decree, DateTime date)
        {
            string dat = date.ToString("yyyy-MM-dd");

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM OtherLiving WHERE ID = '" + ID + "';", globalConnection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            var myData = dt.Select();

            MySqlCommand insertSTarc = new MySqlCommand();
            insertSTarc.Connection = globalConnection;
            insertSTarc.CommandText = "INSERT INTO Archive VALUES('" + myData[0].ItemArray[1].ToString() + "', '" + myData[0].ItemArray[2].ToString() + "', '" + myData[0].ItemArray[3].ToString() + "', '" + myData[0].ItemArray[0].ToString() + "', '" + myData[0].ItemArray[4].ToString() + "', '" + decree + "', '" + dat + "');";
            insertSTarc.ExecuteNonQuery();

            delete_FromRoom(globalConnection, myData[0].ItemArray[5].ToString());

            MySqlCommand delete = new MySqlCommand("DELETE FROM OtherLiving WHERE ID = '" + ID + "';", globalConnection);
            delete.ExecuteNonQuery();
        }

        public static void evictionOLE(MySqlConnection globalConnection, string ID, string decree, DateTime date)
        {
            string dat = date.ToString("yyyy-MM-dd");

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM OtherLiving WHERE ID = '" + ID + "';", globalConnection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            var myData = dt.Select();

            MySqlCommand insertSTarc = new MySqlCommand();
            insertSTarc.Connection = globalConnection;
            insertSTarc.CommandText = "INSERT INTO Archive VALUES('" + myData[0].ItemArray[1].ToString() + "', '" + myData[0].ItemArray[2].ToString() + "', '" + myData[0].ItemArray[3].ToString() + "', '" + myData[0].ItemArray[0].ToString() + "', '" + myData[0].ItemArray[4].ToString() + "', '" + decree + "', '" + dat + "');";
            insertSTarc.ExecuteNonQuery();

            MySqlCommand delete = new MySqlCommand("DELETE FROM OtherLiving WHERE ID = '" + ID + "';", globalConnection);
            delete.ExecuteNonQuery();
        }

        public static ObservableCollection<livingST> getAllST(MySqlConnection globalConnection)
        {
            ObservableCollection<livingST> resault = new ObservableCollection<livingST>();
            DateTime evictedTill = DateTime.Now;

            MySqlCommand select = new MySqlCommand();
            select.Connection = globalConnection;
            select.CommandText = "SELECT * FROM Students;";
            select.ExecuteNonQuery();

            MySqlDataAdapter da = new MySqlDataAdapter(select);
            DataTable dt = new DataTable();
            da.Fill(dt);
            var myData = dt.Select();

            for(int i = 0; i < myData.Length; i++)
            {
                if (myData[i].ItemArray[16].ToString() == "")
                    evictedTill = DateTime.Now.AddDays(-1);
                else
                    evictedTill = Convert.ToDateTime(myData[i].ItemArray[16]);
                resault.Add(new livingST { id = myData[i].ItemArray[3].ToString(), gender = myData[i].ItemArray[4].ToString(), faculty = myData[i].ItemArray[5].ToString(), course = Convert.ToInt32(myData[i].ItemArray[6]), form = myData[i].ItemArray[12].ToString(), date = Convert.ToDateTime(myData[i].ItemArray[8]), citizenship = myData[i].ItemArray[14].ToString(), evicted = Convert.ToBoolean(myData[i].ItemArray[15]), evictedTillDate = evictedTill, livingDate = Convert.ToDateTime(myData[i].ItemArray[13]) });
            }

            return resault;
        }

        public static ObservableCollection<livingOL> getAllOL(MySqlConnection globalConnection)
        {
            ObservableCollection<livingOL> resault = new ObservableCollection<livingOL>();
            DateTime evictedTill = DateTime.Now;

            MySqlCommand select = new MySqlCommand();
            select.Connection = globalConnection;
            select.CommandText = "SELECT * FROM OtherLiving;";
            select.ExecuteNonQuery();

            MySqlDataAdapter da = new MySqlDataAdapter(select);
            DataTable dt = new DataTable();
            da.Fill(dt);
            var myData = dt.Select();

            for(int i = 0; i < myData.Length; i++)
            {
                if (myData[i].ItemArray[14].ToString() == "")
                    evictedTill = DateTime.Now.AddDays(-1);
                else
                    evictedTill = Convert.ToDateTime(myData[i].ItemArray[14]);
                resault.Add(new livingOL { id = myData[i].ItemArray[0].ToString(), gender = myData[i].ItemArray[4].ToString(), date = Convert.ToDateTime(myData[i].ItemArray[6]), citizenship = myData[i].ItemArray[10].ToString(), evicted = Convert.ToBoolean(myData[i].ItemArray[13]), evictedTillDate = evictedTill, livingDate = Convert.ToDateTime(myData[i].ItemArray[9]) });
            }

            return resault;
        }

    }

        public static class SearchFunctions
        {
            public static ObservableCollection<LivingItem> search_exEvicted(MySqlConnection globalConnection)
            {
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();
                DateTime d = DateTime.Now;
                string today = d.ToString("yyyy-MM-dd H:mm:ss");
                int room = 0;

                MySqlCommand selectST = new MySqlCommand();
                selectST.Connection = globalConnection;
                selectST.CommandText = "SELECT * FROM (SELECT * FROM Students WHERE evicted = 1) AS T WHERE EvictedTill <= '" + today + "';";
                selectST.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(selectST);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for(int i = 0; i < myData.Length; i++)
                {
                        resault.Add(new LivingItem { surname = myData[i].ItemArray[0].ToString(), name = myData[i].ItemArray[1].ToString(), patonimic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), roomNumber = room });
                }

                selectST.CommandText = "SELECT * FROM (SELECT * FROM OtherLiving WHERE evicted = 1) AS T WHERE EvictedTill <= '" + today + "';";
                selectST.ExecuteNonQuery();

                MySqlDataAdapter da1 = new MySqlDataAdapter(selectST);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                var myData1 = dt1.Select();

                for (int i = 0; i < myData1.Length; i++)
                {
                    resault.Add(new LivingItem { id = myData1[i].ItemArray[0].ToString(), surname = myData1[i].ItemArray[1].ToString(), name = myData1[i].ItemArray[2].ToString(), patonimic = myData1[i].ItemArray[3].ToString(), roomNumber = room });
                }

                    return resault;
            }

            public static ObservableCollection<LivingItem> search_byFIOall(MySqlConnection globalConnection, string fullString, bool onlyOther)
            {
                string[] splitFIO = fullString.Split(new char[] { ' ' });
                string Surname = "", Name = "", Patronimic = "";
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();

                switch (splitFIO.Length)
                {
                    case 1: Surname = splitFIO[0]; resault = onlySurnameSerch(globalConnection, Surname, onlyOther); break;
                    case 2: Surname = splitFIO[0]; Name = splitFIO[1]; resault = surnameNameSearch(globalConnection, Surname, Name, onlyOther); break;
                    case 3: Surname = splitFIO[0]; Name = splitFIO[1]; Patronimic = splitFIO[2]; resault = fullFIOsearch(globalConnection, Surname, Name, Patronimic); break;
                }

                return resault;
            }

            private static ObservableCollection<LivingItem> onlySurnameSerch(MySqlConnection globalConnection, string surname, bool onlyOther)
            {
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();
                int room = 0;

                if (!onlyOther)
                {
                    MySqlCommand surnameSearhST = new MySqlCommand();
                    surnameSearhST.Connection = globalConnection;
                    surnameSearhST.CommandText = "SELECT * FROM Students WHERE Surname = '" + surname + "';";
                    surnameSearhST.ExecuteNonQuery();

                    MySqlDataAdapter da1 = new MySqlDataAdapter(surnameSearhST);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);
                    var myData1 = dt1.Select();

                    for (int i = 0; i < myData1.Length; i++)
                    {
                        if (myData1[i].ItemArray[7].ToString() == "")
                            room = 0;
                        else
                            room = Convert.ToInt32(myData1[i].ItemArray[7]);
                        resault.Add(new LivingItem { surname = myData1[i].ItemArray[0].ToString(), name = myData1[i].ItemArray[1].ToString(), patonimic = myData1[i].ItemArray[2].ToString(), id = myData1[i].ItemArray[3].ToString(), roomNumber = room });
                    }
                }

                MySqlCommand surnameSerchOL = new MySqlCommand();
                surnameSerchOL.Connection = globalConnection;
                surnameSerchOL.CommandText = "SELECT * FROM OtherLiving WHERE Surname = '" + surname + "';";
                surnameSerchOL.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(surnameSerchOL);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for (int i = 0; i < myData.Length; i++)
                {
                    if (myData[i].ItemArray[5].ToString() == "")
                        room = 0;
                    else
                        room = Convert.ToInt32(myData[i].ItemArray[5]);
                    resault.Add(new LivingItem { id = myData[i].ItemArray[0].ToString(), surname = myData[i].ItemArray[1].ToString(), name = myData[i].ItemArray[2].ToString(), patonimic = myData[i].ItemArray[3].ToString(), roomNumber = room });
                }

                return resault;
            }

            private static ObservableCollection<LivingItem> surnameNameSearch(MySqlConnection globalConnection, string surname, string name, bool onlyOther)
            {
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();
                int room = 0;

                if (!onlyOther)
                {
                    MySqlCommand searchST = new MySqlCommand();
                    searchST.Connection = globalConnection;
                    searchST.CommandText = "SELECT * FROM Students WHERE Surname = '" + surname + "' AND Name = '" + name + "';";
                    searchST.ExecuteNonQuery();

                    MySqlDataAdapter das = new MySqlDataAdapter(searchST);
                    DataTable dts = new DataTable();
                    das.Fill(dts);
                    var myData = dts.Select();

                    if (myData.Length == 0)
                    {
                        searchST.CommandText = "SELECT * FROM Students Where Surname = '" + name + "' AND Name = '" + surname + "';";
                        searchST.ExecuteNonQuery();
                        MySqlDataAdapter das1 = new MySqlDataAdapter(searchST);
                        DataTable dts1 = new DataTable();
                        das1.Fill(dts1);
                        myData = dts1.Select();
                    }

                    if (myData.Length == 0)
                    {
                        searchST.CommandText = "SELECT * FROM Students WHERE Surname = '" + surname + "';";
                        searchST.ExecuteNonQuery();
                        MySqlDataAdapter das2 = new MySqlDataAdapter(searchST);
                        DataTable dts2 = new DataTable();
                        das2.Fill(dts2);
                        myData = dts2.Select();
                    }

                    if (myData.Length == 0)
                    {
                        searchST.CommandText = "SELECT * FROM Students WHERE Surname = '" + name + "';";
                        searchST.ExecuteNonQuery();
                        MySqlDataAdapter das3 = new MySqlDataAdapter(searchST);
                        DataTable dts3 = new DataTable();
                        das3.Fill(dts3);
                        myData = dts3.Select();
                    }
                    for (int i = 0; i < myData.Length; i++)
                    {
                        if (myData[i].ItemArray[7].ToString() == "")
                            room = 0;
                        else
                            room = Convert.ToInt32(myData[i].ItemArray[7]);
                        resault.Add(new LivingItem { surname = myData[i].ItemArray[0].ToString(), name = myData[i].ItemArray[1].ToString(), patonimic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), roomNumber = room });
                    }
                }

                MySqlCommand serchOL = new MySqlCommand();
                serchOL.Connection = globalConnection;
                serchOL.CommandText = "SELECT * FROM OtherLiving WHERE Surname = '" + surname + "' AND Name = '" + name + "';";
                serchOL.ExecuteNonQuery();

                MySqlDataAdapter dao = new MySqlDataAdapter(serchOL);
                DataTable dto = new DataTable();
                dao.Fill(dto);
                var myData1 = dto.Select();

                if (myData1.Length == 0)
                {
                    serchOL.CommandText = "SELECT * FROM OtherLiving WHERE Surname = '" + name + "' AND Name = '" + surname + "';";
                    serchOL.ExecuteNonQuery();

                    MySqlDataAdapter dao1 = new MySqlDataAdapter(serchOL);
                    dao1.Fill(dto);
                    myData1 = dto.Select();
                }

                if (myData1.Length == 0)
                {
                    serchOL.CommandText = "SELECT * FROM OtherLiving WHERE Surname = '" + surname + "';";
                    serchOL.ExecuteNonQuery();

                    MySqlDataAdapter dao2 = new MySqlDataAdapter(serchOL);
                    dao2.Fill(dto);
                    myData1 = dto.Select();
                }

                if (myData1.Length == 0)
                {
                    serchOL.CommandText = "SELECT * FROM OtherLiving WHERE Surname = '" + name + "';";
                    serchOL.ExecuteNonQuery();

                    MySqlDataAdapter dao3 = new MySqlDataAdapter(serchOL);
                    dao3.Fill(dto);
                    myData1 = dto.Select();
                }

                for (int i = 0; i < myData1.Length; i++)
                {
                    if(myData1[i].ItemArray[5].ToString() == "")
                        room = 0;
                    else
                        room = Convert.ToInt32(myData1[i].ItemArray[5]);
                    resault.Add(new LivingItem { id = myData1[i].ItemArray[0].ToString(), surname = myData1[i].ItemArray[1].ToString(), name = myData1[i].ItemArray[2].ToString(), patonimic = myData1[i].ItemArray[3].ToString(), roomNumber = room });
                }

                return resault;
            }

            private static ObservableCollection<LivingItem> fullFIOsearch(MySqlConnection globalconnection, string surname, string name, string patronimic)
            {
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();
                int fl = -1;
                int room = 0;

                MySqlCommand searchOL = new MySqlCommand();
                searchOL.Connection = globalconnection;

                MySqlCommand searchST = new MySqlCommand();
                searchST.Connection = globalconnection;
                searchST.CommandText = "SELECT * FROM Students WHERE Name = '" + name + "' AND Surname = '" + surname + "' AND Patronymic = '" + patronimic + "';";
                searchST.ExecuteNonQuery();

                MySqlDataAdapter das = new MySqlDataAdapter(searchST);
                DataTable dts = new DataTable();
                das.Fill(dts);
                var myData = dts.Select();
                if (myData.Length != 0)
                    fl = 1;

                searchOL.CommandText = "SELECT * FROM OtherLiving WHERE Name = '" + name + "' AND Surname = '" + surname + "' AND Patronymic = '" + patronimic + "';";
                searchOL.ExecuteNonQuery();

                MySqlDataAdapter dao = new MySqlDataAdapter(searchOL);
                dao.Fill(dts);
                var myData1 = dts.Select();
                if (myData.Length == 0 && myData1.Length != 0)
                    fl = 0;


                if (myData.Length == 0 && myData1.Length == 0)
                {
                    searchST.CommandText = "SELECT * FROM Students WHERE Name = '" + name + "' AND Surname = '" + patronimic + "' AND Patronymic = '" + surname + "';";
                    searchST.ExecuteNonQuery();

                    MySqlDataAdapter dat1 = new MySqlDataAdapter(searchST);
                    dat1.Fill(dts);
                    myData = dts.Select();
                    fl = 1;
                }

                if (myData.Length == 0 && myData1.Length == 0)
                {
                    searchST.CommandText = "SELECT * FROM Students WHERE Name = '" + surname + "' AND Surname = '" + name + "' AND Patronymic = '" + patronimic + "';";
                    searchST.ExecuteNonQuery();

                    MySqlDataAdapter dat2 = new MySqlDataAdapter(searchST);
                    dat2.Fill(dts);
                    myData = dts.Select();
                    fl = 1;
                }

                if (myData.Length == 0 && myData1.Length == 0)
                {
                    searchST.CommandText = "SELECT * FROM Students WHERE Name = '" + surname + "' AND Surname = '" + patronimic + "' AND Patronymic = '" + patronimic + "';";
                    searchST.ExecuteNonQuery();

                    MySqlDataAdapter dat3 = new MySqlDataAdapter(searchST);
                    dat3.Fill(dts);
                    myData = dts.Select();
                    fl = 1;
                }

                if (myData.Length == 0 && myData1.Length == 0)
                {
                    searchST.CommandText = "SELECT * FROM Students WHERE Name = '" + patronimic + "' AND Surname = '" + name + "' AND Patronymic = '" + surname + "';";
                    searchST.ExecuteNonQuery();

                    MySqlDataAdapter dat4 = new MySqlDataAdapter(searchST);
                    dat4.Fill(dts);
                    myData = dts.Select();
                    fl = 1;
                }

                if (myData.Length == 0 && myData1.Length == 0)
                {
                    searchST.CommandText = "SELECT * FROM Students WHERE Name = '" + patronimic + "' AND Surname = '" + surname + "' AND Patronymic = '" + name + "';";
                    searchST.ExecuteNonQuery();

                    MySqlDataAdapter dat5 = new MySqlDataAdapter(searchST);
                    dat5.Fill(dts);
                    myData = dts.Select();
                    fl = 1;
                }

                if (myData.Length == 0 && myData1.Length == 0)
                {
                    searchOL.CommandText = "SELECT * FROM OtherLiving WHERE Name = '" + name + "' AND Surname = '" + patronimic + "' AND Patronymic = '" + patronimic + "';";
                    searchOL.ExecuteNonQuery();

                    MySqlDataAdapter dao1 = new MySqlDataAdapter(searchOL);
                    dao1.Fill(dts);
                    myData1 = dts.Select();
                    fl = 0;
                }

                if (myData.Length == 0 && myData1.Length == 0)
                {
                    searchOL.CommandText = "SELECT * FROM OtherLiving WHERE Name = '" + surname + "' AND Surname = '" + name + "' AND Patronymic = '" + patronimic + "';";
                    searchOL.ExecuteNonQuery();

                    MySqlDataAdapter dao2 = new MySqlDataAdapter(searchOL);
                    dao2.Fill(dts);
                    myData1 = dts.Select();
                    fl = 0;
                }

                if (myData.Length == 0 && myData1.Length == 0)
                {
                    searchOL.CommandText = "SELECT * FROM OtherLiving WHERE Name ='" + surname + "' AND Surname = '" + patronimic + "' AND Patronymic = '" + name + "';";
                    searchOL.ExecuteNonQuery();

                    MySqlDataAdapter dao3 = new MySqlDataAdapter(searchOL);
                    dao3.Fill(dts);
                    myData1 = dts.Select();
                    fl = 0;
                }

                if (myData.Length == 0 && myData1.Length == 0)
                {
                    searchOL.CommandText = "SELECT * FROM OtherLiving WHERE Name ='" + patronimic + "' AND Surname = '" + name + "' AND Patronymic = '" + surname + "';";
                    searchOL.ExecuteNonQuery();

                    MySqlDataAdapter dao4 = new MySqlDataAdapter(searchOL);
                    dao4.Fill(dts);
                    myData1 = dts.Select();
                    fl = 0;
                }

                if (myData.Length == 0 && myData1.Length == 0)
                {
                    searchOL.CommandText = "SELECT * FROM OtherLiving WHERE Name = '" + patronimic + "' AND Surname = '" + surname + "' AND Patronymic = '" + name + "';";
                    searchOL.ExecuteNonQuery();

                    MySqlDataAdapter dao5 = new MySqlDataAdapter(searchOL);
                    dao5.Fill(dts);
                    myData1 = dts.Select();
                    fl = 0;
                }

                if (myData.Length == 0 && myData1.Length == 0)
                { resault = surnameNameSearch(globalconnection, surname, name, false); }
                else
                    if (fl == 0)
                        for (int i = 0; i < myData1.Length; i++)
                        {
                            if (myData1[i].ItemArray[5].ToString() == "")
                                room = 0;
                            else
                                room = Convert.ToInt32(myData1[i].ItemArray[5]);
                            resault.Add(new LivingItem { id = myData1[i].ItemArray[15].ToString(), surname = myData1[i].ItemArray[0].ToString(), name = myData1[i].ItemArray[1].ToString(), patonimic = myData1[i].ItemArray[2].ToString(), roomNumber = room });
                        }
                    else
                        if (fl == 1)
                            for (int i = 0; i < myData.Length; i++)
                            {
                                if (myData1[i].ItemArray[7].ToString() == "")
                                    room = 0;
                                else
                                    room = Convert.ToInt32(myData1[i].ItemArray[7]);
                                resault.Add(new LivingItem { surname = myData[i].ItemArray[0].ToString(), name = myData[i].ItemArray[1].ToString(), patonimic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), roomNumber = room });
                            }
                return resault;
            }

            public static ObservableCollection<LivingItem> search_byCourse(MySqlConnection globalConnection, string course, bool isPayed, string faculty, string form, bool evicted)
            {
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();
                string tooday = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                string command = "";
                if (course != "")
                {
                    int iCourse = Convert.ToInt32(course);

                    if (isPayed == true && faculty == "" && form == "")
                        command = "SELECT * FROM Students WHERE Course = '" + iCourse + "' AND Date < '" + tooday + "' AND evicted = 0;";
                    if (isPayed == true && faculty == "" && form != "")
                        command = "SELECT * FROM Students WHERE Course = '" + iCourse + "' AND Date < '" + tooday + "' AND Form = '" + form + "' AND evicted = 0;";
                    if (isPayed == true && faculty != "" && form == "")
                        command = "SELECT * FROM Students WHERE Course = '" + iCourse + "' AND Date < '" + tooday + "' AND Faculty = '" + faculty + "'AND evicted = 0; ";
                    if (isPayed == true && faculty != "" && form != "")
                        command = "SELECT * FROM Students WHERE Course = '" + iCourse + "' AND Date < '" + tooday + "' AND Faculty = '" + faculty + "' AND Form = '" + form + "' AND evicted = 0;";

                    if (isPayed == false && faculty == "" && form == "" && evicted == false)
                        command = "SELECT * FROM Students WHERE Course = '" + iCourse + "' AND evicted = 0;";
                    if (isPayed == false && faculty == "" && form == "" && evicted == true)
                        command = "SELECT * FROM Students WHERE Course = '" + iCourse + "' AND evicted = 1;";
                    if (isPayed == false && faculty == "" && form != "" && evicted == false)
                        command = "SELECT * FROM Students WHERE Course = '" + iCourse + "' AND Form = '" + form + "' AND evicted = 0;";
                    if (isPayed == false && faculty == "" && form != "" && evicted == true)
                        command = "SELECT * FROM Students WHERE Course = '" + iCourse + "' AND Form = '" + form + "' AND evicted = 1;";
                    if (isPayed == false && faculty != "" && form == "" && evicted == false)
                        command = "SELECT * FROM Students WHERE Course = '" + iCourse + "' AND Faculty = '" + faculty + "' AND evicted = 0;";
                    if (isPayed == false && faculty != "" && form == "" && evicted == true)
                        command = "SELECT * FROM Students WHERE Course = '" + iCourse + "' AND Faculty = '" + faculty + "' AND evicted = 1;";
                    if (isPayed == false && faculty != "" && form != "" && evicted == false)
                        command = "SELECT * FROM Students WHERE Course = '" + iCourse + "' AND Faculty = '" + faculty + "' AND Form = '" + form + "' AND evicted = 0;";
                    if (isPayed == false && faculty != "" && form != "" && evicted == true)
                        command = "SELECT * FROM Students WHERE Course = '" + iCourse + "' AND Faculty = '" + faculty + "' AND Form = '" + form + "' AND evicted = 1;";

                    MySqlCommand courseSearch = new MySqlCommand();
                    courseSearch.Connection = globalConnection;
                    courseSearch.CommandText = command;
                    courseSearch.ExecuteNonQuery();

                    int room = 0;

                    MySqlDataAdapter da = new MySqlDataAdapter(courseSearch);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    var myData = dt.Select();

                    for (int i = 0; i < myData.Length; i++)
                    {
                        if (myData[i].ItemArray[7].ToString() == "")
                            room = 0;
                        else
                            room = Convert.ToInt32(myData[i].ItemArray[7]);
                        if(Convert.ToBoolean(myData[i].ItemArray[15]) == false)
                            resault.Add(new LivingItem { surname = myData[i].ItemArray[0].ToString(), name = myData[i].ItemArray[1].ToString(), patonimic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), roomNumber = room });
                        else
                            resault.Add(new LivingItem { surname = myData[i].ItemArray[0].ToString() + "(*)", name = myData[i].ItemArray[1].ToString(), patonimic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), roomNumber = room });
                    }
                }
                return resault;

                /*
                if (isPayed)
                    resault = search_byCoursePayed(globalConnection, course);
                else
                    resault = search_byCourseFull(globalConnection, course);
                return resault;*/

            }

            public static ObservableCollection<LivingItem> search_ByID(MySqlConnection globalConnection, string ID)
            {
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();

                MySqlCommand id_searchST = new MySqlCommand();
                id_searchST.Connection = globalConnection;
                id_searchST.CommandText = "SELECT * FROM Students WHERE Student_ID = '" + ID + "' AND evicted = 0;";
                id_searchST.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(id_searchST);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for (int i = 0; i < myData.Length; i++)
                {
                    resault.Add(new LivingItem { surname = myData[i].ItemArray[0].ToString(), name = myData[i].ItemArray[1].ToString(), patonimic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), roomNumber = Convert.ToInt32(myData[i].ItemArray[7]) });
                }

                if (myData.Length == 0)
                {
                    MySqlCommand id_searchOL = new MySqlCommand();
                    id_searchOL.Connection = globalConnection;
                    id_searchOL.CommandText = "SELECT * FROM OtherLiving WHERE ID = '" + ID + "' AND evicted = 0;";
                    id_searchOL.ExecuteNonQuery();

                    MySqlDataAdapter da1 = new MySqlDataAdapter(id_searchOL);
                    da1.Fill(dt);
                    var myDataO = dt.Select();

                    for (int i = 0; i < myDataO.Length; i++)
                    {
                        resault.Add(new LivingItem { id = myDataO[i].ItemArray[17].ToString(), surname = myDataO[i].ItemArray[0].ToString(), name = myDataO[i].ItemArray[1].ToString(), patonimic = myDataO[i].ItemArray[2].ToString(), roomNumber = Convert.ToInt32(myDataO[i].ItemArray[7]) });
                    }
                }
                return resault;
            }

            public static ObservableCollection<LivingItem> commonSearch(MySqlConnection globalConnection, bool isPayed, bool isOL, string faculty, string form, bool evicted)
            {
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();
                string commandST = "";
                string commandOL = "";
                string today = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                int room = 0;

                MySqlCommand commonSearchST = new MySqlCommand();
                commonSearchST.Connection = globalConnection;

                MySqlCommand commonSearchOL = new MySqlCommand();
                commonSearchOL.Connection = globalConnection;

                if (isPayed == false && isOL == false && faculty == "" && form == "" && evicted == false)
                {
                    commandST = "SELECT * FROM Students WHERE evicted = 0;";
                    commandOL = "SELECT * FROM OtherLiving WHERE evicted = 0;";
                }

                if (isOL == false && faculty == "" && form == "" && evicted == true)
                {
                    commandST = "SELECT * FROM Students WHERE evicted = 1;";
                    commandOL = "SELECT * FROM OtherLiving WHERE evicted = 1;";
                }

                if (isPayed == true && isOL == true)
                    commandOL = "SELECT * FROM OtherLiving WHERE Date < '" + today + "' AND evicted = 0;";
                if (isPayed == false && isOL == true && evicted == false)
                    commandOL = "SELECT * FROM OtherLiving WHERE evicted = 0;";
                if (isPayed == false && isOL == true && evicted == true)
                    commandOL = "SELECT * FROM OtherLiving WHERE evicted = 0;";

                if (isPayed == true && isOL == false && faculty == "" && form == "")
                {
                    commandST = "SELECT * FROM Students WHERE Date < '" + today + "' AND evicted = 0;";
                    commandOL = "SELECT * FROM OtherLiving WHERE Date < '" + today + "' AND evicted = 0;";
                }
                if (isPayed == true && isOL == false && faculty == "" && form != "")
                    commandST = "SELECT * FROM Students WHERE Date < '" + today + "' AND Form = '" + form + "' AND evicted = 0;";
                if (isPayed == true && isOL == false && faculty != "" && form == "")
                    commandST = "SELECT * FROM Students WHERE Date < '" + today + "' AND Faculty = '" + faculty + "' AND evicted = 0;";
                if (isPayed == true && isOL == false && faculty != "" && form != "")
                    commandST = "SELECT * FROM Students WHERE Date < '" + today + "' AND Faculty = '" + faculty + "' AND Form = '" + form + "' AND evicted = 0;";
                if (isPayed == false && isOL == false && faculty == "" && form != "" && evicted == false)
                    commandST = "SELECT * FROM Students WHERE Form = '" + form + "' AND evicted = 0;";
                if (isPayed == false && isOL == false && faculty == "" && form != "" && evicted == true)
                    commandST = "SELECT * FROM Students WHERE Form = '" + form + "' AND evicted = 1;";
                if (isPayed == false && isOL == false && faculty != "" && form == "" && evicted == false)
                    commandST = "SELECT * FROM Students WHERE Faculty = '" + faculty + "' AND evicted = 0;";
                if (isPayed == false && isOL == false && faculty != "" && form == "" && evicted == true)
                    commandST = "SELECT * FROM Students WHERE Faculty = '" + faculty + "' AND evicted = 1;";
                if (isPayed == false && isOL == false && faculty != "" && form != "" && evicted == false)
                    commandST = "SELECT * FROM Students WHERE Faculty = '" + faculty + "' AND Form = '" + form + "' AND evicted = 0;";
                if (isPayed == false && isOL == false && faculty != "" && form != "" && evicted == true)
                    commandST = "SELECT * FROM Students WHERE Faculty = '" + faculty + "' AND Form = '" + form + "' AND evicted = 1;";

                if (commandOL != "")
                {
                    commonSearchOL.CommandText = commandOL;
                    commonSearchOL.ExecuteNonQuery();
                    MySqlDataAdapter da1 = new MySqlDataAdapter(commonSearchOL);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);
                    var myData1 = dt.Select();

                    for (int i = 0; i < myData1.Length; i++)
                    {
                        if (myData1[i].ItemArray[5].ToString() == "")
                            room = 0;
                        else
                            room = Convert.ToInt32(myData1[i].ItemArray[5]);
                        if (Convert.ToBoolean(myData1[i].ItemArray[13]) == false)
                            resault.Add(new LivingItem { id = myData1[i].ItemArray[0].ToString(), surname = myData1[i].ItemArray[1].ToString(), name = myData1[i].ItemArray[2].ToString(), patonimic = myData1[i].ItemArray[3].ToString(), roomNumber = room });
                        else
                            resault.Add(new LivingItem { id = myData1[i].ItemArray[0].ToString(), surname = myData1[i].ItemArray[1].ToString() + "(*)", name = myData1[i].ItemArray[2].ToString(), patonimic = myData1[i].ItemArray[3].ToString(), roomNumber = room });
                    }
                }

                if (commandST != "")
                {
                    commonSearchST.CommandText = commandST;
                    commonSearchST.ExecuteNonQuery();

                    MySqlDataAdapter da = new MySqlDataAdapter(commonSearchST);
                    DataTable dt1 = new DataTable();
                    da.Fill(dt1);
                    var myData1 = dt1.Select();

                    for (int i = 0; i < myData1.Length; i++)
                    {                            
                        if (myData1[i].ItemArray[7].ToString() == "")
                            room = 0;
                        else
                            room = Convert.ToInt32(myData1[i].ItemArray[7]);
                        if (Convert.ToBoolean(myData1[i].ItemArray[15]) == false)
                            resault.Add(new LivingItem { surname = myData1[i].ItemArray[0].ToString(), name = myData1[i].ItemArray[1].ToString(), patonimic = myData1[i].ItemArray[2].ToString(), id = myData1[i].ItemArray[3].ToString(), roomNumber = room });
                        else
                            resault.Add(new LivingItem { surname = myData1[i].ItemArray[0].ToString() + "(*)", name = myData1[i].ItemArray[1].ToString(), patonimic = myData1[i].ItemArray[2].ToString(), id = myData1[i].ItemArray[3].ToString(), roomNumber = room });
                    }
                }

                return resault;
            }

            public static ObservableCollection<LivingItem> search_ByRoomNumber(MySqlConnection globalConnection, string roomNumber)
            {
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();
                if (roomNumber != "")
                {
                    int iNumber = Convert.ToInt32(roomNumber);

                    MySqlCommand roomST = new MySqlCommand();
                    roomST.Connection = globalConnection;
                    roomST.CommandText = "SELECT * FROM Students WHERE Room = '" + iNumber + "' AND evicted = 0;";
                    roomST.ExecuteNonQuery();

                    MySqlDataAdapter da1 = new MySqlDataAdapter(roomST);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);
                    var myData = dt.Select();

                    for (int i = 0; i < myData.Length; i++)
                    {
                        resault.Add(new LivingItem { surname = myData[i].ItemArray[0].ToString(), name = myData[i].ItemArray[1].ToString(), patonimic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), roomNumber = Convert.ToInt32(myData[i].ItemArray[7]) });
                    }

                    MySqlCommand roomOL = new MySqlCommand();
                    roomOL.Connection = globalConnection;
                    roomOL.CommandText = "SELECT * FROM OtherLiving WHERE Room = '" + iNumber + "' AND evicted = 0;";
                    roomOL.ExecuteNonQuery();

                    MySqlDataAdapter da = new MySqlDataAdapter(roomOL);
                    DataTable dt1 = new DataTable();
                    da.Fill(dt1);
                    var myData1 = dt1.Select();

                    for (int i = 0; i < myData1.Length; i++)
                    {
                        resault.Add(new LivingItem { id = myData1[i].ItemArray[0].ToString(), surname = myData1[i].ItemArray[1].ToString(), name = myData1[i].ItemArray[2].ToString(), patonimic = myData1[i].ItemArray[3].ToString(), roomNumber = Convert.ToInt32(myData1[i].ItemArray[5]) });
                    }
                }
                return resault;
            }

            public static ObservableCollection<LivingItem> search_byStayLimit(MySqlConnection globalConnection, string stayLimit, bool isEnd, bool isOL, string faculty, string form)
            {
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();
                string today = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                string _1 = DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd H:mm:ss");
                string _3 = DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd H:mm:ss");
                string _6 = DateTime.Now.AddMonths(6).ToString("yyyy-MM-dd H:mm:ss");

                string commandST = "";
                string commandOL = "";

                if(isEnd == true && isOL == false && faculty == "" && form == "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit < '" + today + "' AND evicted = 0;";
                    commandOL = "SELECT * FROM OtherLiving WHERE StayLimit < '" + today + "' AND evicted = 0;";
                }

                if(isEnd == true && isOL == true)
                {
                    commandOL = "SELECT * FROM OtherLiving WHERE StayLimit < '" + today + "' AND evicted = 0;";
                }

                if(isEnd == true && faculty != "" && form == "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit < '" + today + "' AND Faculty = '" + faculty + "' AND evicted = 0;";
                }

                if(isEnd == true && faculty == "" && form != "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit < '" + today + "' AND Form = '" + form + "' AND evicted = 0;";
                }

                if(isEnd == true && faculty != "" && form != "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit < '" + today + "' AND Faculty = '" + faculty + "' AND Form = '" + form + "' AND evicted = 0;";
                }

                if(stayLimit == "< 1" && faculty == "" && form == "" && isOL == false)
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + today + "' AND StayLimit < '" + _1 + "' AND evicted = 0;";
                    commandOL = "SELECT * FROM OtherLiving WHERE StayLimit >= '" + today + "' AND StayLimit < '" + _1 + "' AND evicted = 0;";
                }

                if(stayLimit == "< 1" && isOL == true)
                {
                    commandOL = "SELECT * FROM OtherLiving WHERE StayLimit >= '" + today + "' AND StayLimit < '" + _1 + "' AND evicted = 0;";
                }

                if(stayLimit == "< 1" && form != "" && faculty == "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + today + "' AND StayLimit < '" + _1 + "' AND Form = '" + form + "' AND evicted = 0;";
                }

                if(stayLimit == "< 1" && form == "" && faculty != "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + today + "' AND StayLimit < '" + _1 + "' AND Faculty = '" + faculty + "' AND evicted = 0;";
                }

                if(stayLimit == "< 1" && form != "" && faculty != "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + today + "' AND StayLimit < '" + _1 + "' AND Form = '" + form + "' AND Faculty = '" + faculty + "' AND evicted = 0;";
                }

                if(stayLimit == "1 - 3" && isOL == false && faculty == "" && form == "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + _1 + "' AND StayLimit < '" + _3 + "' AND evicted = 0;";
                    commandOL = "SELECT * FROM OtherLiving WHERE StayLimit >= '" + _1 + "' AND StayLimit < '" + _3 + "' AND evicted = 0;";
                }

                if(stayLimit == "1 - 3" && isOL == true)
                {
                    commandOL = "SELECT * FROM OtherLiving WHERE StayLimit >= '" + _1 + "' AND StayLimit < '" + _3 + "' AND evicted = 0;";
                }

                if(stayLimit == "1 - 3" && faculty != "" && form == "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + _1 + "' AND StayLimit < '" + _3 + "' AND Faculty = '" + faculty + "' AND evicted = 0;";
                }

                if(stayLimit == "1 - 3" && faculty == "" && form != "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + _1 + "' AND StayLimit < '" + _3 + "' AND Form = '" + form + "' AND evicted = 0;";
                }

                if(stayLimit == "1 - 3" && faculty != "" && form != "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + _1 + "' AND StayLimit < '" + _3 + "' AND Form = '" + form + "' AND Faculty = '" + faculty + "' AND evicted = 0;";
                }

                if(stayLimit == "3 - 6" && faculty == "" && form == "" && isOL == false)
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + _3 + "' AND StayLimit < '" + _6 + "' AND evicted = 0;";
                    commandOL = "SELECT * FROM OtherLiving Where StayLimit >= '" + _3 + "' AND StayLimit < '" + _6 + "' AND evicted = 0;";
                }

                if(stayLimit == "3 - 6" && isOL == true)
                {
                    commandOL = "SELECT * FROM OtherLiving Where StayLimit >= '" + _3 + "' AND StayLimit < '" + _6 + "' AND evicted = 0;";
                }

                if(stayLimit == "3 - 6" && faculty != "" && form == "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + _3 + "' AND StayLimit < '" + _6 + "' AND Faculty = '" + faculty + "' AND evicted = 0;";
                }

                if(stayLimit == "3 - 6" && faculty == "" && form != "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + _3 + "' AND StayLimit < '" + _6 + "' AND Form = '" + form + "' AND evicted = 0;";
                }

                if(stayLimit == "3 - 6" && faculty != "" && form != "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + _3 + "' AND StayLimit < '" + _6 + "' AND Form = '" + form + "' AND Faculty = '" + faculty + "' AND evicted = 0;";
                }

                if(stayLimit == "> 6" && faculty == "" && form == "" && isOL == false)
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + _6 + "' AND evicted = 0;";
                    commandOL = "SELECT * FROM OtherLiving WHERE StayLimit >= '" + _6 + "' AND evicted = 0;";
                }

                if(stayLimit == "> 6" && isOL == true)
                {
                    commandOL = "SELECT * FROM OtherLiving WHERE StayLimit >= '" + _6 + "' AND evicted = 0;";
                }

                if(stayLimit == "> 6" && faculty != "" && form == "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + _6 + "' AND Faculty = '" + faculty + "' AND evicted = 0;";
                }

                if(stayLimit == "> 6" && faculty == "" && form != "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + _6 + "' AND Form = '" + form + "' AND evicted = 0;";
                }

                if(stayLimit == "> 6" && faculty != "" && form != "")
                {
                    commandST = "SELECT * FROM Students WHERE StayLimit >= '" + _6 + "' AND Form = '" + form + "' AND Faculty = '" + faculty + "' AND evicted = 0;";
                }

                if(commandST != "")
                {
                    MySqlCommand searchST = new MySqlCommand();
                    searchST.Connection = globalConnection;
                    searchST.CommandText = commandST;
                    searchST.ExecuteNonQuery();

                    MySqlDataAdapter da1 = new MySqlDataAdapter(searchST);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);
                    var myData1 = dt1.Select();

                    for(int i = 0 ; i < myData1.Length; i++)
                    {
                        resault.Add(new LivingItem { surname = myData1[i].ItemArray[0].ToString(), name = myData1[i].ItemArray[1].ToString(), patonimic = myData1[i].ItemArray[2].ToString(), id = myData1[i].ItemArray[3].ToString(), roomNumber = Convert.ToInt32(myData1[i].ItemArray[7]) });
                    }
                }

                if(commandOL != "")
                {
                    MySqlCommand searchOL = new MySqlCommand();
                    searchOL.Connection = globalConnection;
                    searchOL.CommandText = commandOL;
                    searchOL.ExecuteNonQuery();

                    MySqlDataAdapter da2 = new MySqlDataAdapter(searchOL);
                    DataTable dt2 = new DataTable();
                    da2.Fill(dt2);
                    var myData2 = dt2.Select();

                    for(int i = 0; i < myData2.Length; i++)
                    {
                        resault.Add(new LivingItem { surname = myData2[i].ItemArray[1].ToString(), name = myData2[i].ItemArray[2].ToString(), patonimic = myData2[i].ItemArray[3].ToString(), id = myData2[i].ItemArray[0].ToString(), roomNumber = Convert.ToInt32(myData2[i].ItemArray[5]) });
                    }
                }

                return resault;

            } 

            public static ObservableCollection<LivingItem> search_ByCitizenship(MySqlConnection globalConnection, string citizenship, bool isPayed, bool isOL, string faculty, string form, bool evicted)
            {
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();
                string commandST = "";
                string commandOL = "";
                string today = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");

                if (isPayed == false && isOL == false && faculty == "" && form == "" && evicted == false)
                {
                    commandOL = "SELECT * FROM OtherLiving WHERE Citizenship = '" + citizenship + "' AND evicted = 0;";
                    commandST = "SELECT * FROM Students WHERE Citizenship = '" + citizenship + "' AND evicted = 0;";
                }

                if (isPayed == false && isOL == false && faculty == "" && form == "" && evicted == true)
                {
                    commandOL = "SELECT * FROM OtherLiving WHERE Citizenship = '" + citizenship + "' AND evicted = 1;";
                    commandST = "SELECT * FROM Students WHERE Citizenship = '" + citizenship + "' AND evicted = 1;";
                }

                if (isPayed == false && isOL == true && evicted == false)
                    commandOL = "SELECT * FROM OtherLiving WHERE Citizenship = '" + citizenship + "' AND evicted = 0;";
                if (isPayed == false && isOL == true && evicted == true)
                    commandOL = "SELECT * FROM OtherLiving WHERE Citizenship = '" + citizenship + "' AND evicted = 1;";
                if (isPayed == true && isOL == true && evicted == false)
                    commandOL = "SELECT * FROM OtherLiving WHERE Citizenship = '" + citizenship + "' AND Date < '" + today + "' AND evicted = 0;";

                if (isPayed == true && isOL == false && faculty == "" && form == "" && evicted == false)
                {
                    commandOL = "SELECT * FROM OtherLiving WHERE Citizenship = '" + citizenship + "' AND Date < '" + today + "' AND evicted = 0;";
                    commandST = "SELECT * FROM Students WHERE Citizenship = '" + citizenship + "' AND Date < '" + today + "' AND evicted = 0;";
                }

                if (isPayed == true && isOL == false && faculty == "" && form != "" && evicted == false)
                    commandST = "SELECT * FROM Students WHERE Citizenship = '" + citizenship + "' AND Date < '" + today + "' AND Form = '" + form + "' AND evicted = 0;";
                if (isPayed == true && isOL == false && faculty != "" && form == "" && evicted == false)
                    commandST = "SELECT * FROM Students WHERE Citizenship = '" + citizenship + "' AND Date < '" + today + "' AND Faculty = '" + faculty + "' AND evicted = 0;";
                if (isPayed == true && isOL == false && faculty != "" && form != "" && evicted == false)
                    commandST = "SELECT * FROM Students WHERE Citizenship = '" + citizenship + "' AND Date < '" + today + "' AND Faculty = '" + faculty + "' AND Form = '" + form + "' AND evicted = 0;";
                if (isPayed == false && isOL == false && faculty == "" && form != "" && evicted == false)
                    commandST = "SELECT * FROM Students WHERE Citizenship = '" + citizenship + "' AND Form = '" + form + "' AND evicted = 0;";
                if (isPayed == false && isOL == false && faculty == "" && form != "" && evicted == true)
                    commandST = "SELECT * FROM Students WHERE Citizenship = '" + citizenship + "' AND Form = '" + form + "' AND evicted = 1;";
                if (isPayed == false && isOL == false && faculty != "" && form == "" && evicted == false)
                    commandST = "SELECT * FROM Students WHERE Citizenship = '" + citizenship + "' AND Faculty = '" + faculty + "' AND evicted = 0;";
                if (isPayed == false && isOL == false && faculty != "" && form == "" && evicted == true)
                    commandST = "SELECT * FROM Students WHERE Citizenship = '" + citizenship + "' AND Faculty = '" + faculty + "' AND evicted = 1;";
                if (isPayed == false && isOL == false && faculty != "" && form != "" && evicted == false)
                    commandST = "SELECT * FROM Students WHERE Citizenship = '" + citizenship + "' AND Faculty = '" + faculty + "' AND Form = '" + form + "' AND evicted = 0;";
                if (isPayed == false && isOL == false && faculty != "" && form != "" && evicted == true)
                    commandST = "SELECT * FROM Students WHERE Citizenship = '" + citizenship + "' AND Faculty = '" + faculty + "' AND Form = '" + form + "' AND evicted = 1;";

                int room = 0;

                if (commandOL != "")
                {
                    MySqlCommand citizenShipSearchOL = new MySqlCommand();
                    citizenShipSearchOL.Connection = globalConnection;
                    citizenShipSearchOL.CommandText = commandOL;
                    citizenShipSearchOL.ExecuteNonQuery();

                    MySqlDataAdapter da1 = new MySqlDataAdapter(citizenShipSearchOL);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);
                    var myData1 = dt1.Select();

                    for (int i = 0; i < myData1.Length; i++)
                    {
                        if (myData1[i].ItemArray[5].ToString() == "")
                            room = 0;
                        else
                            room = Convert.ToInt32(myData1[i].ItemArray[5]);
                        if(Convert.ToBoolean(myData1[i].ItemArray[13]) == false)
                            resault.Add(new LivingItem { id = myData1[i].ItemArray[0].ToString(), surname = myData1[i].ItemArray[1].ToString(), name = myData1[i].ItemArray[2].ToString(), patonimic = myData1[i].ItemArray[3].ToString(), roomNumber = room });
                        else
                            resault.Add(new LivingItem { id = myData1[i].ItemArray[0].ToString(), surname = myData1[i].ItemArray[1].ToString() + "(*)", name = myData1[i].ItemArray[2].ToString(), patonimic = myData1[i].ItemArray[3].ToString(), roomNumber = room });
                    }
                }

                if (commandST != "")
                {
                    MySqlCommand citizenShipSearchST = new MySqlCommand();
                    citizenShipSearchST.Connection = globalConnection;
                    citizenShipSearchST.CommandText = commandST;
                    citizenShipSearchST.ExecuteNonQuery();

                    MySqlDataAdapter da2 = new MySqlDataAdapter(citizenShipSearchST);
                    DataTable dt2 = new DataTable();
                    da2.Fill(dt2);
                    var myData = dt2.Select();

                    for (int i = 0; i < myData.Length; i++)
                    {
                        if (myData[i].ItemArray[7].ToString() == "")
                            room = 0;
                        else
                            room = Convert.ToInt32(myData[i].ItemArray[7]);
                        if(Convert.ToBoolean(myData[i].ItemArray[15]) == false)
                            resault.Add(new LivingItem { surname = myData[i].ItemArray[0].ToString(), name = myData[i].ItemArray[1].ToString(), patonimic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), roomNumber = room });
                        else
                            resault.Add(new LivingItem { surname = myData[i].ItemArray[0].ToString() + "(*)", name = myData[i].ItemArray[1].ToString(), patonimic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), roomNumber = room });
                    }
                }
                return resault;
            }

            public static string convertToFullCitizenship(MySqlConnection globalConnection, string shortName)
            {
                MySqlCommand selectFullName = new MySqlCommand();
                selectFullName.Connection = globalConnection;
                selectFullName.CommandText = "SELECT * FROM Citizenship WHERE Name = '" + shortName + "';";
                selectFullName.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(selectFullName);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                return myData[0].ItemArray[1].ToString();
            }

            public static string convertToShortCitizenship(MySqlConnection globalConnection, string fullName)
            {
                MySqlCommand selectName = new MySqlCommand();
                selectName.Connection = globalConnection;
                selectName.CommandText = "SELECT * FROM Citizenship WHERE FullName = '" + fullName + "';";
                selectName.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(selectName);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                return myData[0].ItemArray[0].ToString();
            }

            public static ObservableCollection<LivingItem> search_ByDecreeNumber(MySqlConnection globalConnection, string decreeNumber)
            {
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();

                MySqlCommand searchST = new MySqlCommand();
                searchST.Connection = globalConnection;
                searchST.CommandText = "SELECT * FROM Students WHERE Decree = '" + decreeNumber + "' AND evicted = 0;";
                searchST.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(searchST);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for (int i = 0; i < myData.Length; i++)
                {
                    resault.Add(new LivingItem { surname = myData[i].ItemArray[0].ToString(), name = myData[i].ItemArray[1].ToString(), patonimic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), roomNumber = Convert.ToInt32(myData[i].ItemArray[7]) });
                }

                if (myData.Length == 0)
                {
                    MySqlCommand searchOL = new MySqlCommand();
                    searchOL.Connection = globalConnection;
                    searchOL.CommandText = "SELECT * FROM OtherLiving WHERE Decree = '" + decreeNumber + "' AND evicted = 0;";
                    searchOL.ExecuteNonQuery();

                    MySqlDataAdapter da1 = new MySqlDataAdapter(searchOL);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);
                    var myData1 = dt1.Select();

                    for (int i = 0; i < myData1.Length; i++)
                    {
                        resault.Add(new LivingItem { id = myData1[i].ItemArray[0].ToString(), surname = myData1[i].ItemArray[1].ToString(), name = myData1[i].ItemArray[2].ToString(), patonimic = myData1[i].ItemArray[3].ToString(), roomNumber = Convert.ToInt32(myData1[i].ItemArray[5]) });
                    }
                }

                return resault;

            }

            public static string[] fill_ID(MySqlConnection globalConnection, string studentID)
            {
                string[] resault = new string[19];
                resault[0] = "S";
                MySqlCommand searchST = new MySqlCommand();
                searchST.Connection = globalConnection;
                searchST.CommandText = "SELECT * FROM Students WHERE Student_ID = '" + studentID + "';";
                searchST.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(searchST);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();
                if (myData.Length != 0)
                {
                    if (Convert.ToBoolean(myData[0].ItemArray[15]) == true)
                        resault[0] = "SE";
                    for (int i = 0; i < myData[0].ItemArray.Length; i++)
                    {
                        resault[i + 1] = myData[0].ItemArray[i].ToString();
                    }
                }

                if (myData.Length == 0)
                {
                    resault[0] = "O";

                    MySqlCommand searchOL = new MySqlCommand();
                    searchOL.Connection = globalConnection;
                    searchOL.CommandText = "SELECT * FROM OtherLiving WHERE ID = '" + studentID + "';";
                    searchOL.ExecuteNonQuery();

                    MySqlDataAdapter da1 = new MySqlDataAdapter(searchOL);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);
                    var myData1 = dt1.Select();
                    if (myData1.Length != 0)
                    {
                        if (Convert.ToBoolean(myData1[0].ItemArray[13]) == true)
                            resault[0] = "OE";
                        for (int i = 0; i < myData1[0].ItemArray.Length; i++)
                        {
                            resault[i + 1] = myData1[0].ItemArray[i].ToString();
                        }
                    }
                }

                return resault;
            }

            public static ObservableCollection<RoomItem> commonRoomSearch(MySqlConnection globalConnection, bool isEmpty, bool freePlaces, bool isFull)
            {
                ObservableCollection<RoomItem> resault = new ObservableCollection<RoomItem>();
                string command = "";
                string translate = "";

                MySqlCommand searchRoom = new MySqlCommand();
                searchRoom.Connection = globalConnection;

                if (isEmpty == true && freePlaces == false && isFull == false)
                {
                    command = "SELECT * FROM Rooms WHERE Places = Fplaces;";
                }
                if (isEmpty == true && freePlaces == true && isFull == false)
                {
                    command = "SELECT * FROM Rooms WHERE Places = Fplaces;";
                }

                if (isEmpty == false && freePlaces == true && isFull == false)
                {
                    command = "SELECT * FROM Rooms WHERE Fplaces > 0;";
                }

                if (isEmpty == false && freePlaces == false && isFull == true)
                {
                    command = "SELECT * FROM Rooms WHERE Fplaces = 0;";
                }

                if (isEmpty == false && freePlaces == false && isFull == false)
                {
                    command = "SELECT * FROM Rooms;";
                }

                searchRoom.CommandText = command;
                searchRoom.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(searchRoom);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for (int i = 0; i < myData.Length; i++)
                {
                    if (myData[i].ItemArray[4].ToString() == "f") translate = "женская";
                    if (myData[i].ItemArray[4].ToString() == "m") translate = "мужская";
                    if (myData[i].ItemArray[4].ToString() == "c") translate = "семейная";
                    if (myData[i].ItemArray[4].ToString() == "") translate = "пустая";
                    resault.Add(new RoomItem() { inc = i + 1, room_Numb = myData[i].ItemArray[1].ToString(), places_Con = myData[i].ItemArray[2].ToString(), Fplaces_Con = myData[i].ItemArray[3].ToString(), sex = translate });
                }

                return resault;
            }

            public static ObservableCollection<RoomItem> searchRoomByNumber(MySqlConnection globalConnection, string number)
            {
                ObservableCollection<RoomItem> resault = new ObservableCollection<RoomItem>();
                string translate = "";
                if (number != "")
                {
                    int iNumber = Convert.ToInt32(number);

                    MySqlCommand searchRoom = new MySqlCommand();
                    searchRoom.Connection = globalConnection;
                    searchRoom.CommandText = "SELECT * FROM Rooms WHERE Number = '" + iNumber + "';";
                    searchRoom.ExecuteNonQuery();

                    MySqlDataAdapter da = new MySqlDataAdapter(searchRoom);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    var myData = dt.Select();

                    for (int i = 0; i < myData.Length; i++)
                    {
                        if (myData[i].ItemArray[4].ToString() == "f") translate = "женская";
                        if (myData[i].ItemArray[4].ToString() == "m") translate = "мужская";
                        if (myData[i].ItemArray[4].ToString() == "c") translate = "семейная";
                        if (myData[i].ItemArray[4].ToString() == "") translate = "пустая";
                        resault.Add(new RoomItem() { inc = i + 1, room_Numb = myData[i].ItemArray[1].ToString(), places_Con = myData[i].ItemArray[2].ToString(), Fplaces_Con = myData[i].ItemArray[3].ToString(), sex = translate });
                    }
                }
                return resault;
            }

            public static ObservableCollection<RoomItem> searchRoom_ByPlaces(MySqlConnection globalConnection, string places, bool isEmpty, bool freePlaces, bool isFull)
            {
                ObservableCollection<RoomItem> resault = new ObservableCollection<RoomItem>();
                if (places != "")
                {
                    int iPlaces = Convert.ToInt32(places);
                    string command = "";
                    string translate = "";

                    MySqlCommand searchRoom = new MySqlCommand();
                    searchRoom.Connection = globalConnection;

                    if (isEmpty == false && freePlaces == false && isFull == false)
                    {
                        command = "SELECT * FROM Rooms WHERE Places = '" + iPlaces + "';";
                    }

                    if (isEmpty == true && freePlaces == false && isFull == false)
                    {
                        command = "SELECT * FROM Rooms WHERE Fplaces = Places AND Places = '" + iPlaces + "';";
                    }
                    if (isEmpty == true && freePlaces == true && isFull == false)
                    {
                        command = "SELECT * FROM Rooms WHERE Fplaces = Places AND Places = '" + iPlaces + "';";
                    }

                    if (isEmpty == false && freePlaces == true && isFull == false)
                    {
                        command = "SELECT * FROM Rooms WHERE Fplaces > 0 AND Places = '" + iPlaces + "';";
                    }

                    if (isEmpty == false && freePlaces == false && isFull == true)
                    {
                        command = "SELECT * FROM Rooms WHERE Places = '" + iPlaces + "' AND Fplaces = 0;";
                    }

                    searchRoom.CommandText = command;
                    searchRoom.ExecuteNonQuery();

                    MySqlDataAdapter da = new MySqlDataAdapter(searchRoom);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    var myData = dt.Select();

                    for (int i = 0; i < myData.Length; i++)
                    {
                        if (myData[i].ItemArray[4].ToString() == "f") translate = "женская";
                        if (myData[i].ItemArray[4].ToString() == "m") translate = "мужская";
                        if (myData[i].ItemArray[4].ToString() == "c") translate = "семейная";
                        if (myData[i].ItemArray[4].ToString() == "") translate = "пустая";
                        resault.Add(new RoomItem() { inc = i + 1, room_Numb = myData[i].ItemArray[1].ToString(), places_Con = myData[i].ItemArray[2].ToString(), Fplaces_Con = myData[i].ItemArray[3].ToString(), sex = translate });
                    }
                }
                return resault;
            }

            public static ObservableCollection<RoomItem> searchRoom_ByFplaces(MySqlConnection globalConnection, string fplaces, string places)
            {
                ObservableCollection<RoomItem> resault = new ObservableCollection<RoomItem>();

                if (fplaces != "")
                {
                    int ifPlaces = Convert.ToInt32(fplaces);
                    int iPlaces = Convert.ToInt32(places);
                    string command = "";
                    string translate = "";

                    MySqlCommand searchRoom = new MySqlCommand();
                    searchRoom.Connection = globalConnection;

                    if (iPlaces == 0)
                    {
                        command = "SELECT * FROM Rooms WHERE Fplaces = '" + ifPlaces + "';";
                    }
                    else
                    {
                        command = "SELECT * FROM Rooms WHERE Fplaces = '" + ifPlaces + "' AND Places = '" + iPlaces + "';";
                    }

                    searchRoom.CommandText = command;
                    searchRoom.ExecuteNonQuery();

                    MySqlDataAdapter da = new MySqlDataAdapter(searchRoom);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    var myData = dt.Select();

                    for (int i = 0; i < myData.Length; i++)
                    {
                        if (myData[i].ItemArray[4].ToString() == "f") translate = "женская";
                        if (myData[i].ItemArray[4].ToString() == "m") translate = "мужская";
                        if (myData[i].ItemArray[4].ToString() == "c") translate = "семейная";
                        if (myData[i].ItemArray[4].ToString() == "") translate = "пустая";
                        resault.Add(new RoomItem() { inc = i + 1, room_Numb = myData[i].ItemArray[1].ToString(), places_Con = myData[i].ItemArray[2].ToString(), Fplaces_Con = myData[i].ItemArray[3].ToString(), sex = translate });
                    }
                }
                return resault;
            }

            public static ObservableCollection<RoomItem> searchRoom_ByType(MySqlConnection globalConnection, string type, bool freePlaces, bool isFull)
            {
                ObservableCollection<RoomItem> resault = new ObservableCollection<RoomItem>();
                string command = "";
                string translate = "";

                MySqlCommand searchRoom = new MySqlCommand();
                searchRoom.Connection = globalConnection;

                if (freePlaces == false && isFull == false)
                {
                    command = "SELECT * FROM Rooms WHERE Gender = '" + type + "';";
                }
                if (freePlaces == true && isFull == false)
                {
                    command = "SELECT * FROM Rooms WHERE Gender = '" + type + "' AND Fplaces > 0;";
                }
                if (freePlaces == false && isFull == true)
                {
                    command = "SELECT * FROM Rooms WHERE Gender = '" + type + "' AND Fplaces = 0;";
                }

                searchRoom.CommandText = command;
                searchRoom.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(searchRoom);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for (int i = 0; i < myData.Length; i++)
                {
                    if (myData[i].ItemArray[4].ToString() == "f") translate = "женская";
                    if (myData[i].ItemArray[4].ToString() == "m") translate = "мужская";
                    if (myData[i].ItemArray[4].ToString() == "c") translate = "семейная";
                    if (myData[i].ItemArray[4].ToString() == "") translate = "пустая";
                    resault.Add(new RoomItem() { inc = i + 1, room_Numb = myData[i].ItemArray[1].ToString(), places_Con = myData[i].ItemArray[2].ToString(), Fplaces_Con = myData[i].ItemArray[3].ToString(), sex = translate });
                }

                return resault;
            }


            /*private static ObservableCollection<LivingItem> search_byCoursePayed(MySqlConnection globalConnection, string course)
            {
                int iCourse = Convert.ToInt32(course);
                string tooday = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();

                MySqlCommand courseSearch = new MySqlCommand();
                courseSearch.Connection = globalConnection;
                courseSearch.CommandText = "SELECT * FROM Students WHERE Course = '" + iCourse + "' AND Date < '" + tooday + "';";
                courseSearch.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(courseSearch);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for(int i = 0; i < myData.Length; i++)
                {
                    resault.Add(new LivingItem { surname = myData[i].ItemArray[0].ToString(), name = myData[i].ItemArray[1].ToString(), patonimic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), roomNumber = Convert.ToInt32(myData[i].ItemArray[7]) });
                }
                return resault;
            }*/

            /*private static ObservableCollection<LivingItem> search_byCourseFull(MySqlConnection globalConnection, string course)
            {
                int iCourse = Convert.ToInt32(course);
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();

                MySqlCommand courseSearch = new MySqlCommand();
                courseSearch.Connection = globalConnection;
                courseSearch.CommandText = "SELECT * FROM Students WHERE Course = '" + iCourse + "';";
                courseSearch.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(courseSearch);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for (int i = 0; i < myData.Length; i++)
                {
                    resault.Add(new LivingItem { surname = myData[i].ItemArray[0].ToString(), name = myData[i].ItemArray[1].ToString(), patonimic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), roomNumber = Convert.ToInt32(myData[i].ItemArray[7]) });
                }
                return resault;
            }*/
        }

        public class LivingItem
        {
            public string name { get; set; }
            public string surname { get; set; }
            public string patonimic { get; set; }
            public string id { get; set; }
            public int roomNumber { get; set; }

            public static ObservableCollection<LivingItem> showAllLiving(MySqlConnection globalConnection)
            {
                ObservableCollection<LivingItem> resault = new ObservableCollection<LivingItem>();

                MySqlCommand selectStudents = new MySqlCommand();
                selectStudents.Connection = globalConnection;
                selectStudents.CommandText = "SELECT * FROM Students WHERE evicted = 0;";
                selectStudents.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(selectStudents);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for (int i = 0; i < myData.Length; i++)
                {
                    resault.Add(new LivingItem { surname = myData[i].ItemArray[0].ToString(), name = myData[i].ItemArray[1].ToString(), patonimic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), roomNumber = Convert.ToInt32(myData[i].ItemArray[7]) });
                }

                MySqlCommand selectOL = new MySqlCommand();
                selectOL.Connection = globalConnection;
                selectOL.CommandText = "SELECT * FROM OtherLiving WHERE evicted = 0;";
                selectOL.ExecuteNonQuery();

                MySqlDataAdapter da1 = new MySqlDataAdapter(selectOL);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                var MyData1 = dt1.Select();

                for (int i = 0; i < MyData1.Length; i++)
                {
                    resault.Add(new LivingItem { id = MyData1[i].ItemArray[0].ToString(), surname = MyData1[i].ItemArray[1].ToString(), name = MyData1[i].ItemArray[2].ToString(), patonimic = MyData1[i].ItemArray[3].ToString(), roomNumber = Convert.ToInt32(MyData1[i].ItemArray[5]) });
                }

                return resault;
            }

        }

        public class RoomItem
        {
            public int inc { get; set; }
            public string room_Numb { get; set; }
            public string places_Con { get; set; }
            public string Fplaces_Con { get; set; }
            public string sex { get; set; }

            public static ObservableCollection<RoomItem> getItems(MySqlConnection connection, string s)
            {
                ObservableCollection<RoomItem> resault = new ObservableCollection<RoomItem>();
                string empt = "";
                MySqlCommand selectRoom = new MySqlCommand();
                selectRoom.Connection = connection;
                selectRoom.CommandText = "SELECT * FROM Rooms WHERE (((Gender = '" + s + "') OR (Gender = '" + empt + "')) AND (Fplaces > 0));";
                selectRoom.ExecuteNonQuery();

                MySqlDataAdapter adapter = new MySqlDataAdapter(selectRoom);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                var myData = dt.Select();
                string translate = "";

                for (int i = 0; i < myData.Length; i++)
                {
                    if (myData[i].ItemArray[4].ToString() == "f") translate = "женская";
                    if (myData[i].ItemArray[4].ToString() == "m") translate = "мужская";
                    if (myData[i].ItemArray[4].ToString() == "c") translate = "семейная";
                    if (myData[i].ItemArray[4].ToString() == "") translate = "пустая";
                    resault.Add(new RoomItem() { inc = i + 1, room_Numb = myData[i].ItemArray[1].ToString(), places_Con = myData[i].ItemArray[2].ToString(), Fplaces_Con = myData[i].ItemArray[3].ToString(), sex = translate });
                }

                return resault;
            }

            public static ObservableCollection<RoomItem> getItems(MySqlConnection globalConnection)
            {
                ObservableCollection<RoomItem> resault = new ObservableCollection<RoomItem>();
                string empt = "";
                MySqlCommand selectRoom = new MySqlCommand();
                selectRoom.Connection = globalConnection;
                selectRoom.CommandText = "SELECT * FROM Rooms;";
                selectRoom.ExecuteNonQuery();

                MySqlDataAdapter adapter = new MySqlDataAdapter(selectRoom);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                var myData = dt.Select();
                string translate = "";

                for (int i = 0; i < myData.Length; i++)
                {
                    if (myData[i].ItemArray[4].ToString() == "f") translate = "женская";
                    if (myData[i].ItemArray[4].ToString() == "m") translate = "мужская";
                    if (myData[i].ItemArray[4].ToString() == "c") translate = "семейная";
                    if (myData[i].ItemArray[4].ToString() == "") translate = "пустая";
                    resault.Add(new RoomItem() { inc = i + 1, room_Numb = myData[i].ItemArray[1].ToString(), places_Con = myData[i].ItemArray[2].ToString(), Fplaces_Con = myData[i].ItemArray[3].ToString(), sex = translate });
                }

                return resault;
            }

            public static ObservableCollection<RoomItem> getItems(MySqlConnection connection, string s, int placesCount)
            {
                ObservableCollection<RoomItem> resault = new ObservableCollection<RoomItem>();
                string empt = "";
                MySqlCommand selectRoom = new MySqlCommand();
                selectRoom.Connection = connection;
                selectRoom.CommandText = "SELECT * FROM Rooms WHERE (((Gender = '" + s + "') OR (Gender = '" + empt + "')) AND (Fplaces > 0) AND (Places = '" + placesCount + "'));";
                selectRoom.ExecuteNonQuery();

                MySqlDataAdapter adapter = new MySqlDataAdapter(selectRoom);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                var myData = dt.Select();
                string translate = "";

                for (int i = 0; i < myData.Length; i++)
                {
                    if (myData[i].ItemArray[4].ToString() == "f") translate = "женская";
                    if (myData[i].ItemArray[4].ToString() == "m") translate = "мужская";
                    if (myData[i].ItemArray[4].ToString() == "c") translate = "семейная";
                    if (myData[i].ItemArray[4].ToString() == "") translate = "пустая";
                    resault.Add(new RoomItem() { inc = i + 1, room_Numb = myData[i].ItemArray[1].ToString(), places_Con = myData[i].ItemArray[2].ToString(), Fplaces_Con = myData[i].ItemArray[3].ToString(), sex = translate });
                }

                return resault;
            }

            public static ObservableCollection<RoomItem> src_OnlyFreeRooms(MySqlConnection connection)
            {
                ObservableCollection<RoomItem> resault = new ObservableCollection<RoomItem>();
                string empt = String.Empty;
                MySqlCommand select_OnlyFreeRooms = new MySqlCommand();
                select_OnlyFreeRooms.Connection = connection;
                select_OnlyFreeRooms.CommandText = "SELECT * FROM Rooms WHERE Gender = '" + empt + "' ORDER BY Number;";
                select_OnlyFreeRooms.ExecuteNonQuery();

                MySqlDataAdapter adapter = new MySqlDataAdapter(select_OnlyFreeRooms);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                var myData = dt.Select();
                string translate = "пустая";

                for (int i = 0; i < myData.Length; i++)
                    resault.Add(new RoomItem() { inc = i + 1, room_Numb = myData[i].ItemArray[1].ToString(), places_Con = myData[i].ItemArray[2].ToString(), Fplaces_Con = myData[i].ItemArray[3].ToString(), sex = translate });
                return resault;
            }

            public static ObservableCollection<RoomItem> src_OnlyFreeRooms(MySqlConnection connection, int placesCount)
            {
                ObservableCollection<RoomItem> resault = new ObservableCollection<RoomItem>();
                string empt = "";
                MySqlCommand selectRoom = new MySqlCommand();
                selectRoom.Connection = connection;
                selectRoom.CommandText = "SELECT * FROM Rooms WHERE Gender = '" + empt + "' AND Places = '" + placesCount + "';";
                selectRoom.ExecuteNonQuery();

                MySqlDataAdapter adapter = new MySqlDataAdapter(selectRoom);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                var myData = dt.Select();
                string translate = "";

                for (int i = 0; i < myData.Length; i++)
                {
                    if (myData[i].ItemArray[4].ToString() == "f") translate = "женская";
                    if (myData[i].ItemArray[4].ToString() == "m") translate = "мужская";
                    if (myData[i].ItemArray[4].ToString() == "c") translate = "семейная";
                    if (myData[i].ItemArray[4].ToString() == "") translate = "пустая";
                    resault.Add(new RoomItem() { inc = i + 1, room_Numb = myData[i].ItemArray[1].ToString(), places_Con = myData[i].ItemArray[2].ToString(), Fplaces_Con = myData[i].ItemArray[3].ToString(), sex = translate });
                }

                return resault;
            }

            public static ObservableCollection<RoomItem> src_OnlyFamilyRooms(MySqlConnection connection)
            {
                ObservableCollection<RoomItem> resault = new ObservableCollection<RoomItem>();
                string empt = "c";
                MySqlCommand select_OnlyFamilyRooms = new MySqlCommand();
                select_OnlyFamilyRooms.Connection = connection;
                select_OnlyFamilyRooms.CommandText = "SELECT * FROM Rooms WHERE Gender = '" + empt + "' ORDER BY Number;";
                select_OnlyFamilyRooms.ExecuteNonQuery();

                MySqlDataAdapter adapter = new MySqlDataAdapter(select_OnlyFamilyRooms);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                var myData = dt.Select();
                string translate = "семейная";

                for (int i = 0; i < myData.Length; i++)
                    resault.Add(new RoomItem() { inc = i + 1, room_Numb = myData[i].ItemArray[1].ToString(), places_Con = myData[i].ItemArray[2].ToString(), Fplaces_Con = myData[i].ItemArray[3].ToString(), sex = translate });
                return resault;
            }

            public static ObservableCollection<RoomItem> src_ByNumber(MySqlConnection connection, string Numbs, string s)
            {
                ObservableCollection<RoomItem> resault = new ObservableCollection<RoomItem>();

                int Numb = Convert.ToInt32(Numbs);
                string empt = String.Empty;
                string familyRooms = "c";

                MySqlCommand srcNumber = new MySqlCommand();
                srcNumber.Connection = connection;
                srcNumber.CommandText = "SELECT * FROM Rooms WHERE (((Gender = '" + s + "')OR (Gender = '" + empt + "')) OR ((Gender = '" + familyRooms + "') AND (Fplaces > 0))) AND Number = '" + Numb + "';";
                srcNumber.ExecuteNonQuery();

                MySqlDataAdapter adapter = new MySqlDataAdapter(srcNumber);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                var myData = dt.Select();
                string translate = "";

                for (int i = 0; i < myData.Length; i++)
                {
                    if (myData[i].ItemArray[4].ToString() == "f") translate = "женская";
                    if (myData[i].ItemArray[4].ToString() == "m") translate = "мужская";
                    if (myData[i].ItemArray[4].ToString() == "c") translate = "семейная";
                    if (myData[i].ItemArray[4].ToString() == "") translate = "пустая";
                    resault.Add(new RoomItem() { inc = i + 1, room_Numb = myData[i].ItemArray[1].ToString(), places_Con = myData[i].ItemArray[2].ToString(), Fplaces_Con = myData[i].ItemArray[3].ToString(), sex = translate });
                }

                return resault;
            }



        }

        public class archive
        {
            public string name { get; set; }
            public string surname { get; set; }
            public string patronymic { get; set; }
            public string id { get; set; }
            public string date { get; set; }

            public static ObservableCollection<archive> viewArchive(MySqlConnection globalConnection)
            {
                ObservableCollection<archive> resault = new ObservableCollection<archive>();
                string t = "";

                MySqlCommand viewArc = new MySqlCommand();
                viewArc.Connection = globalConnection;
                viewArc.CommandText = "SELECT * FROM Archive;";
                viewArc.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(viewArc);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for (int i = 0; i < myData.Length; i++)
                {
                    if (myData[i].ItemArray[6].ToString() != "")
                    {
                        t = (Convert.ToDateTime(myData[i].ItemArray[6].ToString()).Date).ToString();
                        t = DateTime.Parse(t).ToShortDateString();
                    }
                    /*t = (Convert.ToDateTime(myData[i].ItemArray[6].ToString()).Day).ToString() + "." + (Convert.ToDateTime(myData[i].ItemArray[6].ToString()).Month).ToString() + "." + (Convert.ToDateTime(myData[i].ItemArray[6].ToString()).Year).ToString();*/
                    else
                        t = "";

                    resault.Add(new archive { surname = myData[i].ItemArray[0].ToString(), name = myData[i].ItemArray[1].ToString(), patronymic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), date = t });
                }

                return resault;
            }

            public static ObservableCollection<archive> search_ByID(MySqlConnection globalConnection, string ID)
            {
                ObservableCollection<archive> resault = new ObservableCollection<archive>();
                string t = "";

                MySqlCommand search = new MySqlCommand();
                search.Connection = globalConnection;
                search.CommandText = "SELECT * FROM Archive WHERE Student_ID = '" + ID + "';";
                search.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(search);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for(int i = 0; i < myData.Length; i++)
                {
                    if (myData[i].ItemArray[6].ToString() != "")
                    {
                        t = (Convert.ToDateTime(myData[i].ItemArray[6].ToString()).Date).ToString();
                        t = DateTime.Parse(t).ToShortDateString();
                    }
                    else
                        t = "";

                    resault.Add(new archive { name = myData[i].ItemArray[1].ToString(), surname = myData[i].ItemArray[0].ToString(), patronymic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), date = t });
                }

                return resault;
            }

            public static ObservableCollection<archive> search_ByFIO(MySqlConnection globalConnection, string FIO)
            {
                string[] splitFIO = FIO.Split(new char[] { ' ' });
                string Surname = "", Name = "", Patronimic = "";
                ObservableCollection<archive> resault = new ObservableCollection<archive>();

                switch (splitFIO.Length)
                {
                    case 1: Surname = splitFIO[0]; resault = onlySurnameSerch(globalConnection, Surname); break;
                    case 2: Surname = splitFIO[0]; Name = splitFIO[1]; resault = surnameNameSearch(globalConnection, Surname, Name); break;
                    case 3: Surname = splitFIO[0]; Name = splitFIO[1]; Patronimic = splitFIO[2]; resault = fullFIOsearch(globalConnection, Surname, Name, Patronimic); break;
                }

                return resault;
            }

            private static ObservableCollection<archive> onlySurnameSerch(MySqlConnection globalConnection, string surname)
            {
                ObservableCollection<archive> resault = new ObservableCollection<archive>();
                string t = "";

                    MySqlCommand surnameSearhST = new MySqlCommand();
                    surnameSearhST.Connection = globalConnection;
                    surnameSearhST.CommandText = "SELECT * FROM Archive WHERE Surname = '" + surname + "';";
                    surnameSearhST.ExecuteNonQuery();

                    MySqlDataAdapter da1 = new MySqlDataAdapter(surnameSearhST);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);
                    var myData1 = dt1.Select();

                    for (int i = 0; i < myData1.Length; i++)
                    {
                        if (myData1[i].ItemArray[6].ToString() != "")
                        {
                            t = (Convert.ToDateTime(myData1[i].ItemArray[6].ToString()).Date).ToString();
                            t = DateTime.Parse(t).ToShortDateString();
                        }
                        else
                            t = "";
                        resault.Add(new archive { name = myData1[i].ItemArray[1].ToString(), surname = myData1[i].ItemArray[0].ToString(), patronymic = myData1[i].ItemArray[2].ToString(), id = myData1[i].ItemArray[3].ToString(), date = t });
                    }

                return resault;
            }

            private static ObservableCollection<archive> surnameNameSearch(MySqlConnection globalConnection, string surname, string name)
            {
                ObservableCollection<archive> resault = new ObservableCollection<archive>();
                string t = "";
               
                    MySqlCommand searchST = new MySqlCommand();
                    searchST.Connection = globalConnection;
                    searchST.CommandText = "SELECT * FROM Archive WHERE Surname = '" + surname + "' AND Name = '" + name + "';";
                    searchST.ExecuteNonQuery();

                    MySqlDataAdapter das = new MySqlDataAdapter(searchST);
                    DataTable dts = new DataTable();
                    das.Fill(dts);
                    var myData = dts.Select();

                    if (myData.Length == 0)
                    {
                        searchST.CommandText = "SELECT * FROM Archive Where Surname = '" + name + "' AND Name = '" + surname + "';";
                        searchST.ExecuteNonQuery();
                        MySqlDataAdapter das1 = new MySqlDataAdapter(searchST);
                        DataTable dts1 = new DataTable();
                        das1.Fill(dts1);
                        myData = dts1.Select();
                    }

                    if (myData.Length == 0)
                    {
                        searchST.CommandText = "SELECT * FROM Archive WHERE Surname = '" + surname + "';";
                        searchST.ExecuteNonQuery();
                        MySqlDataAdapter das2 = new MySqlDataAdapter(searchST);
                        DataTable dts2 = new DataTable();
                        das2.Fill(dts2);
                        myData = dts2.Select();
                    }

                    if (myData.Length == 0)
                    {
                        searchST.CommandText = "SELECT * FROM Archive WHERE Surname = '" + name + "';";
                        searchST.ExecuteNonQuery();
                        MySqlDataAdapter das3 = new MySqlDataAdapter(searchST);
                        DataTable dts3 = new DataTable();
                        das3.Fill(dts3);
                        myData = dts3.Select();
                    }
                    for (int i = 0; i < myData.Length; i++)
                    {
                        if (myData[i].ItemArray[6].ToString() != "")
                        {
                            t = (Convert.ToDateTime(myData[i].ItemArray[6].ToString()).Date).ToString();
                            t = DateTime.Parse(t).ToShortDateString();
                        }
                        else
                            t = "";
                        resault.Add(new archive { name = myData[i].ItemArray[1].ToString(), surname = myData[i].ItemArray[0].ToString(), patronymic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), date = t });
                    }
   
                return resault;
            }


            private static ObservableCollection<archive> fullFIOsearch(MySqlConnection globalconnection, string surname, string name, string patronimic)
            {
                ObservableCollection<archive> resault = new ObservableCollection<archive>();
                string t = "";

                MySqlCommand searchOL = new MySqlCommand();
                searchOL.Connection = globalconnection;

                MySqlCommand searchST = new MySqlCommand();
                searchST.Connection = globalconnection;
                searchST.CommandText = "SELECT * FROM Archive WHERE Name = '" + name + "' AND Surname = '" + surname + "' AND Patronymic = '" + patronimic + "';";
                searchST.ExecuteNonQuery();

                MySqlDataAdapter das = new MySqlDataAdapter(searchST);
                DataTable dts = new DataTable();
                das.Fill(dts);
                var myData = dts.Select();

                if (myData.Length == 0)
                {
                    searchST.CommandText = "SELECT * FROM Archive WHERE Name = '" + name + "' AND Surname = '" + patronimic + "' AND Patronymic = '" + surname + "';";
                    searchST.ExecuteNonQuery();

                    MySqlDataAdapter dat1 = new MySqlDataAdapter(searchST);
                    dat1.Fill(dts);
                    myData = dts.Select();
                }

                if (myData.Length == 0)
                {
                    searchST.CommandText = "SELECT * FROM Archive WHERE Name = '" + surname + "' AND Surname = '" + name + "' AND Patronymic = '" + patronimic + "';";
                    searchST.ExecuteNonQuery();

                    MySqlDataAdapter dat2 = new MySqlDataAdapter(searchST);
                    dat2.Fill(dts);
                    myData = dts.Select();
                }

                if (myData.Length == 0)
                {
                    searchST.CommandText = "SELECT * FROM Archive WHERE Name = '" + surname + "' AND Surname = '" + patronimic + "' AND Patronymic = '" + patronimic + "';";
                    searchST.ExecuteNonQuery();

                    MySqlDataAdapter dat3 = new MySqlDataAdapter(searchST);
                    dat3.Fill(dts);
                    myData = dts.Select();
                }

                if (myData.Length == 0)
                {
                    searchST.CommandText = "SELECT * FROM Archive WHERE Name = '" + patronimic + "' AND Surname = '" + name + "' AND Patronymic = '" + surname + "';";
                    searchST.ExecuteNonQuery();

                    MySqlDataAdapter dat4 = new MySqlDataAdapter(searchST);
                    dat4.Fill(dts);
                    myData = dts.Select();
                }

                if (myData.Length == 0)
                {
                    searchST.CommandText = "SELECT * FROM Archive WHERE Name = '" + patronimic + "' AND Surname = '" + surname + "' AND Patronymic = '" + name + "';";
                    searchST.ExecuteNonQuery();

                    MySqlDataAdapter dat5 = new MySqlDataAdapter(searchST);
                    dat5.Fill(dts);
                    myData = dts.Select();
                }

                if (myData.Length == 0)
                { resault = surnameNameSearch(globalconnection, surname, name); }
                else
                    for (int i = 0; i < myData.Length; i++)
                    {
                        if (myData[i].ItemArray[6].ToString() != "")
                        {
                            t = (Convert.ToDateTime(myData[i].ItemArray[6].ToString()).Date).ToString();
                            t = DateTime.Parse(t).ToShortDateString();
                        }
                        else
                            t = "";
                        resault.Add(new archive { name = myData[i].ItemArray[1].ToString(), surname = myData[i].ItemArray[0].ToString(), patronymic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), date = t });
                    }
                return resault;
            }

            public static ObservableCollection<archive> search_ByDate(MySqlConnection globalConnection, DateTime firstDate1, DateTime secondDate1)
            {
                string t = "";
                ObservableCollection<archive> resault = new ObservableCollection<archive>();

                string firstDate = firstDate1.ToString("yyyy-MM-dd H:mm:ss");
                string secondDate = secondDate1.ToString("yyyy-MM-dd H:mm:ss");

                MySqlCommand search = new MySqlCommand();
                search.Connection = globalConnection;
                search.CommandText = "SELECT * FROM Archive WHERE Date >= '" + firstDate + "' AND Date <= '" + secondDate + "'";
                search.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(search);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for(int i = 0; i < myData.Length; i++)
                {
                    if (myData[i].ItemArray[6].ToString() != "")
                    {
                        t = (Convert.ToDateTime(myData[i].ItemArray[6].ToString()).Date).ToString();
                        t = DateTime.Parse(t).ToShortDateString();
                    }
                    else
                        t = "";
                    resault.Add(new archive { name = myData[i].ItemArray[1].ToString(), surname = myData[i].ItemArray[0].ToString(), patronymic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), date = t });
                }

                return resault;
            }

            public static ObservableCollection<archive> search_ByDecree(MySqlConnection globalConnection, string Decree)
            {
                string t = "";
                ObservableCollection<archive> resault = new ObservableCollection<archive>();

                MySqlCommand search = new MySqlCommand();
                search.Connection = globalConnection;
                search.CommandText = "SELECT * FROM Archive WHERE Decree = '" + Decree + "';";
                search.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(search);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for (int i = 0; i < myData.Length; i++) 
                {
                    if (myData[i].ItemArray[6].ToString() != "")
                    {
                        t = (Convert.ToDateTime(myData[i].ItemArray[6].ToString()).Date).ToString();
                        t = DateTime.Parse(t).ToShortDateString();
                    }
                    else
                        t = "";

                    resault.Add(new archive { name = myData[i].ItemArray[1].ToString(), surname = myData[i].ItemArray[0].ToString(), patronymic = myData[i].ItemArray[2].ToString(), id = myData[i].ItemArray[3].ToString(), date = t });
                }
                return resault;
            }
        }

        public class livingST
        {
            public string id { get; set; }
            public string gender { get; set; }
            public string faculty { get; set; }
            public int course { get; set; }
            public string form { get; set; }
            public DateTime date { get; set; }
            public string citizenship { get; set; }
            public bool evicted { get; set; }
            public DateTime evictedTillDate { get; set; }
            public DateTime livingDate { get; set; }
        }

        public class livingOL
        {
            public string id { get; set; }
            public string gender { get; set; }
            public DateTime date { get; set; }
            public string citizenship { get; set; }
            public bool evicted { get; set; }
            public DateTime evictedTillDate { get; set; }
            public DateTime livingDate { get; set; }
        }

        public class roomInfo
        {
            public string housing { get; set; }
            public string room_Number { get; set; }
            public string places_Count { get; set; }
            public string fPlaces_Count { get; set; }
            public string type { get; set; }

            public static ObservableCollection<roomInfo> viewRoomInfo (MySqlConnection globalConnection)
            {
                ObservableCollection<roomInfo> resault = new ObservableCollection<roomInfo>();

                MySqlCommand selectRooms = new MySqlCommand();
                selectRooms.Connection = globalConnection;
                selectRooms.CommandText = "SELECT * FROM Rooms;";
                selectRooms.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(selectRooms);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var myData = dt.Select();

                for(int i = 0; i < myData.Length; i++)
                {
                    resault.Add(new roomInfo { housing = myData[i].ItemArray[0].ToString(), room_Number = myData[i].ItemArray[1].ToString(), places_Count = myData[i].ItemArray[2].ToString(), fPlaces_Count = myData[i].ItemArray[3].ToString(), type = myData[i].ItemArray[4].ToString() });
                }

                return resault;

            }
        }
    }



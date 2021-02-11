using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using BookMyFood.ClientFuncion;
using BookMyFood.Common;
using RestSharp.Extensions;

namespace BookMyFood.Model
{
    public static class SQLiteHelper
    {
        const string defaultBaseFile = "ClientBase.db";



        public static List<T> GetReader<T>(string Table, string Query = "SELECT * FROM ", int IdDeliverer = -1) where T : new()
        {



            if (!File.Exists(defaultBaseFile))
            {
                Log.Instance.W(new { }, "База данных не обнаружена, создаём типовую.");
                Properties.Resources.ClientBase.SaveAs(@".\ClientBase.db");
              
            }



            Log.Instance.W(new {}, $"Получаем данные из таблицы '{Table}'. Запрос: '{Query}'");

                using (SQLiteConnection Connect =
                    new SQLiteConnection(@"Data Source=.\" + defaultBaseFile + "; Version=3;"))
                {
                    try
                    {
                        List<T> Entities = new List<T>();
                    Connect.Open();

                    using (SQLiteCommand cmd = Connect.CreateCommand())
                        {
                          

                            cmd.CommandText = $"{Query} {Table}";

                            if (typeof(T) == typeof(Item)) cmd.CommandText += " where ID_Deliverer = "+IdDeliverer;

                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                            
                            
                                while (reader.Read())
                                {
                                    if (reader["Id"]!=DBNull.Value && !string.IsNullOrEmpty(reader["Id"].ToString()))
                                    {
                                        int Id = 0;
                                        int.TryParse(reader["Id"].ToString(), out Id);

                                        if(typeof(T)==typeof(Deliverer))
                                        {
                                            var item = new T();
                                            (item as Deliverer).ID = Id;
                                            (item as Deliverer).Name = reader["Name"].ToString();
                                            (item as Deliverer).Description = reader["Description"].ToString();
                                            Entities.Add(item);
                                        }
                                        if (typeof(T) == typeof(Item))
                                        {
                                            var item = new T();
                                            (item as Item).ID = Id;
                                            (item as Item).Price = decimal.Parse(reader["Price"].ToString());
                                        (item as Item).Name = reader["Name"].ToString();
                                            (item as Item).Description = reader["Description"].ToString();
                                            Entities.Add(item);
                                        }




                                }

                                }

                            
                            }
                        }

                        return Entities;
                    }
                    catch (Exception e)
                    {
                        Log.Instance.W(new {}, "Ошибка при обращении к SQLite: "+ e.Message);
                        return new List<T>();

                    }
                }

            
        }
    }
}

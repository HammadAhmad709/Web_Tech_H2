using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace LibraryData
{
    public class MemberRepository
    {
        private string connectionString;

        public MemberRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int AddMember(Member member)
        {
            string query = "INSERT INTO Members (Name) VALUES (@Name)";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", member.Name);
            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();
            return (i > 0 ? i : 0);
        }

        public List<Member> GetAllMembers()
        {
            List<Member> members = new List<Member>();
            string query = "SELECT * FROM Members";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                members.Add(new Member
                {
                    MemberId = (int)reader["MemberId"],
                    Name = reader["Name"].ToString()
                });
            }
            conn.Close();
            return members;
        }

    }
}

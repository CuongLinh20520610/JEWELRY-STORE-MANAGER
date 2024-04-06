using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using SE104.Models;
using System.Configuration;
using System.Data;


namespace SE104.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhaCungCap_Controller : ControllerBase
    {
        private readonly string _connectionString;

        public NhaCungCap_Controller(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet("DanhSachNhaCungCap")]
        public IEnumerable<NhaCungCap> GetNhaCungCap()
        {
            List<NhaCungCap> listNhaCungCap = new List<NhaCungCap>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT MaNCC, TenNCC, DiaChi, SDT FROM NHACUNGCAP";
                using (MySqlCommand command = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            NhaCungCap nhaCungCap = new NhaCungCap
                            {
                                MaNCC = reader.GetString("MaNCC"),
                                TenNCC = reader.GetString("TenNCC"),
                                DiaChi = reader.GetString("DiaChi"),
                                SDT = reader.GetString("SDT")
                            };
                            listNhaCungCap.Add(nhaCungCap);
                        }
                    }
                }
            }
            return listNhaCungCap;
        }

        [HttpPost("ThemNhaCungCap")]
        public IActionResult ThemNhaCungCap([FromBody] NhaCungCap nhaCungCap)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string insertNhaCungCapQuery = "INSERT INTO NHACUNGCAP (MaNCC, TenNCC, DiaChi, SDT) VALUES (@MaNCC, @TenNCC, @DiaChi, @SDT)";
                        MySqlCommand insertNhaCungCapCmd = new MySqlCommand(insertNhaCungCapQuery, conn, transaction);
                        insertNhaCungCapCmd.Parameters.AddWithValue("@MaNCC", nhaCungCap.MaNCC);
                        insertNhaCungCapCmd.Parameters.AddWithValue("@TenNCC", nhaCungCap.TenNCC);
                        insertNhaCungCapCmd.Parameters.AddWithValue("@DiaChi", nhaCungCap.DiaChi);
                        insertNhaCungCapCmd.Parameters.AddWithValue("@SDT", nhaCungCap.SDT);
                        insertNhaCungCapCmd.ExecuteNonQuery();

                        transaction.Commit();

                        return Ok("Thêm nhà cung cấp thành công.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return StatusCode(500, $"Lỗi khi thêm nhà cung cấp: {ex.Message}");
                    }
                }
            }
        }
        [HttpDelete("XoaNhaCungCap/{MaNCC}")]
        public IActionResult XoaNhaCungCap(string MaNCC)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string deleteNhaCungCapQuery = "DELETE FROM NHACUNGCAP WHERE MaNCC = @MaNCC";
                        MySqlCommand deleteNhaCungCapCmd = new MySqlCommand(deleteNhaCungCapQuery, conn, transaction);
                        deleteNhaCungCapCmd.Parameters.AddWithValue("@MaNCC", MaNCC);
                        deleteNhaCungCapCmd.ExecuteNonQuery();

                        transaction.Commit();

                        return Ok("Xóa nhà cung cấp thành công.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return StatusCode(500, $"Lỗi khi xóa nhà cung cấp: {ex.Message}");
                    }
                }
            }
        }
    }
}

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
    public class PhieuBanHang_Controller : ControllerBase
    {
        private readonly string _connectionString;
        public PhieuBanHang_Controller(IConfiguration connection)
        {
            _connectionString = connection.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(connection));
        }

        [HttpGet("DanhSachPhieuBanHang")]
        public IEnumerable<PhieuBanHang> Get()
        {
            List<PhieuBanHang> listPhieuBanHang = new List<PhieuBanHang>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT SoPhieu, NgayLap FROM PHIEUBANHANG";
                using (MySqlCommand command = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PhieuBanHang phieubanhang = new PhieuBanHang
                            {
                                SoPhieu = reader.GetString("SoPhieu"),
                                NgayLap = reader.GetDateTime("NgayLap")
                            };
                            listPhieuBanHang.Add(phieubanhang);
                        }
                    }
                }
            }
            return listPhieuBanHang;
        }

        [HttpPost("ThemPhieuBanHang")]
        public IActionResult ThemPhieuBanHang([FromBody] PhieuBanHang phieuBanHang)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string checkExistQuery = "SELECT COUNT(SoPhieu) FROM PHIEUBANHANG WHERE SoPhieu = @SoPhieu";
                        MySqlCommand checkExistCmd = new MySqlCommand(checkExistQuery, conn, transaction);
                        checkExistCmd.Parameters.AddWithValue("@SoPhieu", phieuBanHang.SoPhieu);
                        int count = Convert.ToInt32(checkExistCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            return BadRequest("Phiếu bán hàng đã tồn tại.");
                        }

                        string insertPhieuBanHangQuery = "INSERT INTO PHIEUBANHANG (SoPhieu, NgayLap, MaKH) VALUES (@SoPhieu, @NgayLap, @MaKH)";
                        MySqlCommand insertPhieuBanHangCmd = new MySqlCommand(insertPhieuBanHangQuery, conn, transaction);
                        insertPhieuBanHangCmd.Parameters.AddWithValue("@SoPhieu", phieuBanHang.SoPhieu);
                        insertPhieuBanHangCmd.Parameters.AddWithValue("@NgayLap", phieuBanHang.NgayLap);
                        insertPhieuBanHangCmd.Parameters.AddWithValue("@MaKH", phieuBanHang.MaKH);
                        insertPhieuBanHangCmd.ExecuteNonQuery();

                        transaction.Commit();

                        return Ok("Thêm phiếu bán hàng thành công.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return StatusCode(500, $"Lỗi khi thêm phiếu bán hàng: {ex.Message}");
                    }
                }
            }
        }

        [HttpDelete("XoaPhieuBanHang/{SoPhieu}")]
        public IActionResult XoaPhieuBanHang(string soPhieu)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string deleteCTPBHQuery = "DELETE FROM CTPBH WHERE SoPhieu = @SoPhieu";
                        MySqlCommand deleteCTPBHQCmd = new MySqlCommand(deleteCTPBHQuery, conn, transaction);
                        deleteCTPBHQCmd.Parameters.AddWithValue("@SoPhieu", soPhieu);
                        deleteCTPBHQCmd.ExecuteNonQuery();

                        string deletePhieuBanHangQuery = "DELETE FROM PHIEUBANHANG WHERE SoPhieu = @SoPhieu";
                        MySqlCommand deletePhieuBanHangCmd = new MySqlCommand(deletePhieuBanHangQuery, conn, transaction);
                        deletePhieuBanHangCmd.Parameters.AddWithValue("@SoPhieu", soPhieu);
                        deletePhieuBanHangCmd.ExecuteNonQuery();

                        transaction.Commit();

                        return Ok("Xóa phiếu bán hàng thành công.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return StatusCode(500, $"Lỗi khi xóa phiếu bán hàng: {ex.Message}");
                    }
                }
            }
        }
    }
}

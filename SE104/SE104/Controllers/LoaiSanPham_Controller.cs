using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using SE104.Models;

namespace SE104.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiSanPhamController : ControllerBase
    {
        private readonly string _connectionString;

        public LoaiSanPhamController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet("{MaLSP}")]
        public IActionResult GetTenLoaiSanPham(string MaLSP)
        {
            string tenLoaiSanPham = "";

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT TenLSP FROM LOAISANPHAM WHERE MaLSP = @MaLSP";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaLSP", MaLSP);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    tenLoaiSanPham = result.ToString();
                }
                else
                {
                    return NotFound(); 
                }
            }

            return Ok(tenLoaiSanPham); 
        }

        [HttpPost("ThemLoaiSanPham")]
        public IActionResult ThemLoaiSanPham([FromBody] LoaiSanPham loaiSanPham)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO LOAISANPHAM (MaLSP, TenLSP, MaDVT, PhanTramLN) VALUES (@MaLSP, @TenLSP, @MaDVT, @PhanTramLN)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaLSP", loaiSanPham.MaLSP);
                cmd.Parameters.AddWithValue("@TenLSP", loaiSanPham.TenLSP);
                cmd.Parameters.AddWithValue("@MaDVT", loaiSanPham.MaDVT);
                cmd.Parameters.AddWithValue("@PhanTramLN", loaiSanPham.PhanTramLN);
                cmd.ExecuteNonQuery();
            }

            return Ok("Thêm loại sản phẩm thành công.");
        }

        [HttpDelete("XoaNhaCungCap/{MaLSP}")]
        public IActionResult XoaLoaiSanPham(string MaLSP)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM LOAISANPHAM WHERE MaLSP = @MaLSP";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaLSP", MaLSP);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound(); 
                }
            }

            return Ok("Xóa loại sản phẩm thành công.");
        }
    }
}

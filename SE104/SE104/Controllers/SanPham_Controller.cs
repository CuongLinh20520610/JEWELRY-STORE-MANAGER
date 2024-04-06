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
    public class SanPhamController : ControllerBase
    {
        private readonly string _connectionString;

        public SanPhamController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet("{MaSP}")]
        public IActionResult Get(string MaSP)
        {
            List<SanPham> listSanPham = new List<SanPham>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT MaSP, TenSP, DonGia, TonKho FROM SANPHAM WHERE MaSP = @MaSP";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaSP", MaSP);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SanPham sanPham = new SanPham
                        {
                            MaSP = reader.GetString("MaSP"),
                            TenSP = reader.GetString("TenSP"),
                            DonGia = reader.GetFloat("DonGia"),
                            TonKho = reader.GetInt32("TonKho")
                        };
                        listSanPham.Add(sanPham);
                    }
                }
            }

            if (listSanPham.Count == 0)
            {
                return NotFound();
            }

            return Ok(listSanPham);
        }

        [HttpGet("DanhSachSanPham")]
        public IActionResult GetAllSanPham()
        {
            List<SanPham> listSanPham = new List<SanPham>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT MaSP, TenSP, DonGia, TonKho FROM SANPHAM";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SanPham sanPham = new SanPham
                        {
                            MaSP = reader.GetString("MaSP"),
                            TenSP = reader.GetString("TenSP"),
                            DonGia = reader.GetFloat("DonGia"),
                            TonKho = reader.GetInt32("TonKho")
                        };
                        listSanPham.Add(sanPham);
                    }
                }
            }

            if (listSanPham.Count == 0)
            {
                return NotFound();
            }

            return Ok(listSanPham);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Web;

namespace WebQuanLyBanHoa.Models
{
    [MetadataTypeAttribute(typeof(ThanhVienMetadata))]
    public partial class ThanhVien
    {
        internal sealed class ThanhVienMetadata
        {
			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_MaThanhVien", AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
			[DisplayName("Mã thành viên")]
			public int MaThanhVien
			{
				get;
				set;
			}

			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_TaiKhoan", DbType = "NVarChar(100)")]
			[DisplayName("Tài Khoản")]
			[Required(ErrorMessage = "{0} không được để trống")]

			public string TaiKhoan
			{
				get;
				set;
			}

			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_MatKhau", DbType = "NVarChar(100)")]
			[DisplayName("Mật khẩu")]
			//[Range(6, 36, ErrorMessage = "{0} phải từ {1} đến {2} ký tự")]
			[Required(ErrorMessage = "{0} không được để trống")]
			public string MatKhau
			{
				get;
				set;
			}

			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_HoTen", DbType = "NVarChar(255)")]
			[DisplayName("Họ tên")]
			[Required(ErrorMessage = "{0} không được để trống")]
			public string HoTen
			{
				get;
				set;
			}

			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_DiaChi", DbType = "NVarChar(255)")]
			[DisplayName("Địa chỉ")]
			[Required(ErrorMessage = "{0} không được để trống")]
			public string DiaChi
			{
				get;
				set;
			}

			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_SoDienThoai", DbType = "Char(12)")]
			[DisplayName("Số điện thoại")]
			//[Range(9, 11, ErrorMessage = "{0} phải từ {1} đến {2} ký tự")]
			[StringLength(12, ErrorMessage = "{0} phải có ít nhất {2} số và tối đa là 12 số.", MinimumLength = 10)]
			[Required(ErrorMessage = "{0} không được để trống")]
			
			public string SoDienThoai
			{
				get;
				set;
			}

			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Email", DbType = "Char(100)")]
			[DisplayName("Email")]
			[RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
			ErrorMessage = "Email nhập vào không hợp lệ")]

			//[Required(ErrorMessage = "{0} không được để trống")]
			public string Email
			{
				get;
				set;
			}
		}
    }
}
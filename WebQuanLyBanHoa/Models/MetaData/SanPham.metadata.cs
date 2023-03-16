using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace WebQuanLyBanHoa.Models
{
	[MetadataTypeAttribute(typeof(SanPhamMetadata))]
	public partial class SanPham
    {
		internal sealed class SanPhamMetadata
        {
			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_MaSP", AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
			[DisplayName("Mã SP")]
			public int MaSP
			{
				get;
				set;
			}

			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_TenSP", DbType = "NVarChar(255)")]
			[DisplayName("Tên SP")]
			[Required(ErrorMessage = "{0} không được để trống")]
			public string TenSP
			{
				get;
				set;
			}

			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_DonGia", DbType = "Decimal(18,0)")]
			[DisplayName("Đơn giá")]
			[Required(ErrorMessage = "{0} không được để trống")]
			public System.Nullable<decimal> DonGia
			{
				get;
				set;
			}

			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_NgayCapNhat", DbType = "DateTime")]
			[DisplayName("Ngày cập nhật")]
			[Required(ErrorMessage = "{0} không được để trống")]
			public System.Nullable<System.DateTime> NgayCapNhat
			{
				get;
				set;
			}

			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_SoLuongTon", DbType = "Int")]
			[DisplayName("Số lượng tồn")]
			[Required(ErrorMessage = "{0} không được để trống")]
			public System.Nullable<int> SoLuongTon
			{
				get;
				set;
			}

			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_HinhAnh", DbType = "NVarChar(MAX)")]
			[DisplayName("Hình ảnh")]
			[Required(ErrorMessage = "{0} không được để trống")]
			public string HinhAnh
			{
				get;
				set;
			}

			[global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_MoTa", DbType = "NVarChar(MAX)")]
			[DisplayName("Mô tả")]
			[Required(ErrorMessage = "{0} không được để trống")]
			public string MoTa
			{
				get;
				set;
			}

            [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Moi", DbType = "Int")]
            [DisplayName("Mới")]
            public System.Nullable<int> Moi
            {
                get;
                set;
            }

            [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_DaXoa", DbType = "Bit")]
            [DisplayName("Đã xoá")]
            public System.Nullable<bool> DaXoa
            {
                get;
                set;
            }

            [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_MaNCC", DbType = "Int NOT NULL")]
			[DisplayName("Mã Nhà cung cấp")]
			public int MaNCC
			{
				get;
				set;
			}
		}
	}
}
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Thongtin_LienheRepository: IThongtin_LienheRepository
    {
        private readonly NA_DbContext _context;
        public Thongtin_LienheRepository(NA_DbContext context)
        {
            _context = context;
        }
        public bool Add(Thongtin_Lienhe thongtin)
        {
            try
            {
                _context.Add(thongtin);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool SendMail(Thongtin_Lienhe thongtin)
        {
            try
            {
                var companyEmail = "info.nguyenanhest@gmail.com";
                if (string.IsNullOrEmpty(companyEmail))
                {
                    return false;
                }

                // Cấu hình email
                var fromAddress = new MailAddress("nguyenanhest@gmail.com", "Hệ thống TKB"); 
                var toAddress = new MailAddress(companyEmail, "Công ty");

                MailMessage mail = new MailMessage
                {
                    From = fromAddress,
                    Subject = $"[XEPTHOIKHOABIEU] - {thongtin.Ho_ten} - {thongtin.So_dien_thoai}",
                    Body = $"Người gửi: {thongtin.Ho_ten}\nSố điện thoại: {thongtin.So_dien_thoai}\nEmail: {thongtin.Email}\n" +
                           $"Đơn vị: {thongtin.Ten_don_vi}\nĐịa chỉ: {thongtin.Dia_chi}\nGhi chú: {thongtin.Ghi_chu}",
                    IsBodyHtml = false
                };

                mail.To.Add(toAddress);

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("nguyenanhest@gmail.com", "ozbatorceqgncsyu");
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtp.Send(mail);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

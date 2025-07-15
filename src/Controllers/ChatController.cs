using Microsoft.AspNetCore.Mvc;

namespace Manage_CLB_HTSV.Controllers
{
    public class ChatController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ChatController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        // POST: Chat/ClearHistory
        [HttpPost]
        public IActionResult ClearHistory()
        {
            // Đường dẫn đến file chatHistory.txt
            var messagesDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "messages");
            var filePath = Path.Combine(messagesDirectory, "chatHistory.txt");
            
            // Tạo thư mục nếu không tồn tại
            if (!Directory.Exists(messagesDirectory))
            {
                Directory.CreateDirectory(messagesDirectory);
            }

            try
            {
                // Mở tập tin để ghi và xóa nội dung
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.Write("");
                }

                ViewBag.Message = "Nội dung của tập tin đã được xóa.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Đã xảy ra lỗi: " + ex.Message;
            }

            // Hiển thị lại view Index hoặc trả về JSON response
            return RedirectToAction("Index");
        }
    }
}


using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InternFselV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThreadTestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ThreadTestController(IMediator mediator)
        {
            _mediator = mediator;
        }
        //cở bản
        //tạo Thread là luồng
        [HttpPost]        
        public IActionResult Create()
        {
            Thread t = new Thread(new ThreadStart(delegate {
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine("Hello from Thread");
                }
            }));
            t.Start();
            return Ok();
        }
        //tạo sử dụng Thread.Sleep(); cơ chế dừng luồng trong 1 khoảng tg
        [HttpPost("seconds")]
        public IActionResult CreateInt([FromQuery] int second)
        {
            Thread t = new Thread(new ThreadStart(delegate {
                Console.WriteLine($"Waiting for {second} seconds...");
                Thread.Sleep(second * 1000);
                Console.WriteLine("thanks");
            }));
            t.Start();
            Thread t2 = new Thread(new ThreadStart(delegate {
                Console.WriteLine($"2 Waiting for {second} seconds...");
                Thread.Sleep(second * 1000);
                Console.WriteLine("2 thanks");
            }));
            t2.Start();
            return Ok("thanks");
        }
        // Task cở bản giống Thread nhưng có nhiều tác cơ chết hơn(đợi)
        [HttpPost("total")]
        public Task CreateTotal([FromQuery] int first, [FromQuery] int second)
        {
            int? total = first + second;

            
            return Task.Run(delegate {
                Console.WriteLine("start total ");
                Thread.Sleep(15000);
                Console.WriteLine($"total {first + second}");
            });
        }
        //Trung bình
        //Chia sẻ dữ liệu giữa các Thread : dùng biến chung
        private static int _id = 0;
        [HttpPost("Id")]
        public IActionResult AddId([FromQuery] int id)
        {
            Thread t = new Thread( () => {
                for (int i = 0; i < id; i++)
                {
                    _id = i;
                    //Console.WriteLine($"id = {_id}");
                    Thread.Sleep(1000);
                }
            });
            t.Start();
            return Ok(id);
        }
        [HttpGet("Id")]
        public IActionResult GetId()
        {
            Thread t = new Thread(() => { Console.WriteLine($"Id : {_id}"); });
            t.Start();
            return Ok(_id);
        }
        [HttpPost("IdThread")]
        public IActionResult AddIdThread([FromQuery] int idcon1, [FromQuery] int idcon2)
        {
            int a = 0 ;
            Thread t1 = new Thread(() => { Console.WriteLine($"trước {Thread.CurrentThread.Name}: {a}");a = idcon1; }) { Name = "t1"};
            Thread t2 = new Thread(() => { Console.WriteLine($"trước {Thread.CurrentThread.Name}: {a}");a = idcon2; }) { Name = "t2" };
            t1.Start();
            t1.Join();
            t2.Start();
            Thread.CurrentThread.Name = "Main";
            Console.WriteLine($"{Thread.CurrentThread.Name} : {a}");
            return Ok(a);
        }
        //Tạo và chạy nhiều Task song song: chạy nhiều task cùng lúc
        [HttpGet("totalTask")]
        public IActionResult CreateTotalTask()
        {
            int a = 0;
            Random rnd = new Random();

            Task.Run(() =>
            {
                Console.WriteLine($"statst t1:");
                Thread.Sleep(1000);
                a = rnd.Next()%1000;
                Console.WriteLine($"t1 {a} tổng là: {(a * (a + 1)) / 2}");
            });
            Task.Run(() =>
            {
                Console.WriteLine($"statst t2:");
                Thread.Sleep(2000);
                a = rnd.Next() % 1000;
                Console.WriteLine($"t2 {a} tổng là: {(a * (a + 1)) / 2}");
            });
            Task.Run(() =>
            {
                Console.WriteLine($"statst t3:");
                Thread.Sleep(3000);
                a = rnd.Next() % 1000;
                Console.WriteLine($"t3 {a} tổng là: {(a * (a + 1)) / 2}");
            });
            Task.Run(() =>
            {
                Console.WriteLine($"statst t4:");
                Thread.Sleep(4000);
                a = rnd.Next() % 1000;
                Console.WriteLine($"t4 {a} tổng là: {(a * (a + 1)) / 2}");
            });
            Task.Run(() =>
            {
                Console.WriteLine($"statst t5:");
                Thread.Sleep(5000);
                a = rnd.Next() % 1000;
                Console.WriteLine($"t5 {a} tổng là: {(a * (a + 1)) / 2}");
            });
            return Ok(a);
        }
        //Kết hợp Task với async/await cơ thế đợi( đợ 1 task chạy xong thì mới chạy tiếp) của Task 
        [HttpGet("create3api")]
        public async Task<IActionResult> Create3APIAsync()
        {

            var tasks = new Task[3];
            Console.WriteLine("băt dầu");
            for (int i = 0; i < 3; i++)
            {
                int apiNumber = i + 1;
                tasks[i] = RunApi(apiNumber);
            }
            Console.WriteLine("kết thúc");
            await Task.WhenAll(tasks);

            Console.WriteLine("kết thúc v2");
            return Ok();
        }
        private async Task RunApi(int apiNumber)
        {            
            Task tacVu = new Task(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine($"API {apiNumber}: {i+1}");
                    Task.Delay(500).Wait();
                }
                
            });
            tacVu.Start();
            await tacVu;
            Console.WriteLine($"{apiNumber}thật sao");
        }
        // nâng cao
        //Thread Pool : có chứa sẵn nhiều Thread có sẵn để dùng, tránh việc, khởi tạo và tắt Thread liên tục
        [HttpGet("thread-pool")]
        public IActionResult CreateThreadPool()
        {
            // Create a CountdownEvent with an initial count of 2
            //CountdownEvent countdown = new CountdownEvent(1);
            Stopwatch w = new Stopwatch();
            
            ThreadPool.QueueUserWorkItem(WorkItem1, "Hello");    
            w.Start();
            ThreadPool.QueueUserWorkItem(Sun, Tuple.Create(1000000, w));
            
            
            //countdown.Wait();
            // Wait for the work items to complete


            Console.WriteLine("Main thread exits");
            return Ok();
        }
        private void Sun(object state)
        {
            if (state is Tuple<int, Stopwatch> data)
            {
                int a = 0;
                for(int i = 0;i < (int)data.Item1; i++)
                {
                    a += i;
                }
                Console.WriteLine($"Sun: {a}");
                data.Item2.Stop();
                Console.WriteLine($"Time {data.Item2.ElapsedMilliseconds} Milliseconds");
            }
        }
        // A work item that prints a message
        private void WorkItem1(object state)
        {
            Thread.Sleep(1000);
            Console.WriteLine("WorkItem1: {0}", state);
            /*CountdownEvent countdown = (CountdownEvent)state;
            countdown.Signal();*/
        }
        // Task với kết quả trả về (Task<T>): theo cở chế chờ để láu ra dữ liệu của task( giao cho task(1 luồng) sử lí 1 biến số và lấy lại kết của task đó sau sử lý)
        [HttpGet("create4api")]
        public async Task<IActionResult> Create4API([FromQuery] int first, [FromQuery] int second)
        {

            var tasks = new Task<int>[4];
            var result = new int[4];
            Console.WriteLine("băt dầu");
            tasks[0] = new Task<int>(delegate { return first + second; });
            tasks[1] = new Task<int>(delegate { return first - second; });
            tasks[2] = new Task<int>(delegate { return first * second; });
            tasks[3] = new Task<int>(delegate { return first / second; });
            foreach(var task in tasks)
            {
                task.Start();
            }
            result = await Task.WhenAll(tasks);

            return Ok(result);
        }
        // dừng task : khi cancellationTokent.IsCancellationRequested trả về true tức là dùng
        [HttpGet("stopapi")]
        public async Task<IActionResult> Stopapi([FromQuery] int first)
        {

            var cancellationTokent = new CancellationTokenSource(5000);
            int a = 0;
            Task longRunningTask = new Task(() =>
            {
                a = 0;
                for (int i = 0; i < first; i++)
                {
                    if (cancellationTokent.IsCancellationRequested)
                    {
                        break;
                    }
                    a+= i +1;
                    Console.WriteLine($"a : {a}");
                    Thread.Sleep(500);
                }
            }, cancellationTokent.Token);
            longRunningTask.Start();
            await longRunningTask;

            return Ok(a);
        }
        // dùng Parallel : giống như For/ForEach nhưng chạy nhiều luồn và code chạy lâu việc phân luồng sẽ rút ngăn tg, cẩn thận với việc đáp án của lượt lặp này ảnh hưởng tới lượt lặp sau
        [HttpGet("parallel")]
        public async Task<IActionResult> Darallel()
        {
            var ads = new int[100];
            Parallel.For(0, 100, i => { ads[i] = i; });
            Console.WriteLine("chưa thêm");
            Parallel.ForEach(ads, ad => { Thread.Sleep(500); Console.WriteLine(ad); });
            Console.WriteLine("đã thêm");
            Parallel.ForEach(ads, ad => { ad = ad + 10; Thread.Sleep(1000); Console.WriteLine(ad); });
            return Ok();
        }

    }
}

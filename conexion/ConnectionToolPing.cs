using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace conexion
{
    public class ConnectionToolPing
    {
        
        public List<bool> Result = new List<bool>();
        public async Task Ping(string who)
        {
            var results = new List<bool>();
            AutoResetEvent waiter = new AutoResetEvent (false);

            Ping pingSender = new Ping ();

            // When the PingCompleted event is raised,
            // the PingCompletedCallback method is called.
            pingSender.PingCompleted += new PingCompletedEventHandler (PingCompletedCallback);

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes (data);

            // Wait 12 seconds for a reply.
            int timeout = 12000;

            // Set options for transmission:
            // The data can go through 64 gateways or routers
            // before it is destroyed, and the data packet
            // cannot be fragmented.
            PingOptions options = new PingOptions (64, true);

            Console.WriteLine ("Time to live: {0}", options.Ttl);
            Console.WriteLine ("Don't fragment: {0}", options.DontFragment);

            // Send the ping asynchronously.
            // Use the waiter as the user token.
            // When the callback completes, it can wake up this thread.
            pingSender.SendAsync(who, timeout, buffer, options, waiter);

            // Prevent this example application from ending.
            // A real application should do something useful
            // when possible.
            waiter.WaitOne ();
        }

        private void PingCompletedCallback (object sender, PingCompletedEventArgs e)
        {
            // If the operation was canceled, display a message to the user.
            if (e.Cancelled)
            {
                Console.WriteLine ("Ping canceled.");

                // Let the main thread resume.
                // UserToken is the AutoResetEvent object that the main thread
                // is waiting for.
                ((AutoResetEvent)e.UserState).Set ();
            }

            // If an error occurred, display the exception to the user.
            if (e.Error != null)
            {
                Result.Add(false);
                Console.WriteLine ("Ping failed:");
                Console.WriteLine (e.Error.ToString ());

                // Let the main thread resume.
                ((AutoResetEvent)e.UserState).Set();
            }

            if (!e.Cancelled && e.Error == null)
            {
                Result.Add(true);
            }

            PingReply reply = e.Reply;

            DisplayReply (reply);

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();
        }

        public void DisplayReply (PingReply reply)
        {
            if (reply == null)
                return;

            Console.WriteLine ("ping status: {0}", reply.Status);
            //if (reply.Status == IPStatus.Success)
            //{
            //    Console.WriteLine ("Address: {0}", reply.Address.ToString ());
            //    Console.WriteLine ("RoundTrip time: {0}", reply.RoundtripTime);
            //    Console.WriteLine ("Time to live: {0}", reply.Options.Ttl);
            //    Console.WriteLine ("Don't fragment: {0}", reply.Options.DontFragment);
            //    Console.WriteLine ("Buffer size: {0}", reply.Buffer.Length);
            //}
        }                
    }
}
using System;
using System.IO;
using System.Net;
using System.Text;

namespace WindowsApp1
{

    public static class EventHandler
    {
        public static void test()
        {
            // Create a request using a URL that can receive a post.
            var request = WebRequest.Create("https://api.openai.com/v1/engines/davinci-codex/completions");

            // Set the request method to POST.
            request.Method = "POST";

            string apiKey = "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";

            // Add headers to request
            request.Headers.Add("Authorization", "Bearer " + apiKey);
            request.ContentType = "application/json";

            // Create POST data and convert it to a byte array.
            string postData = "{\"prompt\":\"Translate the following English text to French: '{}'\", \"max_tokens\":60}";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/json";

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            var dataStream = request.GetRequestStream();

            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);

            // Close the Stream object.
            dataStream.Close();

            // Get the response.
            var response = request.GetResponse();

            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            var reader = new StreamReader(dataStream);

            // Read the content.
            string responseFromServer = reader.ReadToEnd();

            // Display the content.
            Console.WriteLine(responseFromServer);

            // Clean up the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();


        }



        // For Each p As Process In Process.GetProcessesByName("Rs500")
        // 'MessageBox.Show(p.ProcessName)
        // If p.ProcessName = "Rs500" Then
        // p.Kill()
        // p.WaitForExit()
        // End If
        // Next


        // Dim path = “C:\Users\37239\OneDrive - Entegris\Desktop\Project\SW2031_W.RSS”
    }
}
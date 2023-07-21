Imports System.IO
Imports System.Net
Imports System.Text

Public Module EventHandler
    Private dataEntries As New Dictionary(Of String, Tuple(Of String, String))
    Private modbusDic As New Dictionary(Of String, List(Of String))
    Private logixApp As Object = CreateObject("RSLogix500.Application")
    Private logixObj As Object
    Public Sub test()
        ' Create a request using a URL that can receive a post.
        Dim request As WebRequest = WebRequest.Create("https://api.openai.com/v1/engines/davinci-codex/completions")

        ' Set the request method to POST.
        request.Method = "POST"

        Dim apiKey As String = "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"

        ' Add headers to request
        request.Headers.Add("Authorization", "Bearer " + apiKey)
        request.ContentType = "application/json"

        ' Create POST data and convert it to a byte array.
        Dim postData As String = "{""prompt"":""Translate the following English text to French: '{}'"", ""max_tokens"":60}"
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)

        ' Set the ContentType property of the WebRequest.
        request.ContentType = "application/json"

        ' Set the ContentLength property of the WebRequest.
        request.ContentLength = byteArray.Length

        ' Get the request stream.
        Dim dataStream As Stream = request.GetRequestStream()

        ' Write the data to the request stream.
        dataStream.Write(byteArray, 0, byteArray.Length)

        ' Close the Stream object.
        dataStream.Close()

        ' Get the response.
        Dim response = request.GetResponse()

        ' Display the status.
        Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)

        dataStream = response.GetResponseStream()

        ' Open the stream using a StreamReader for easy access.
        Dim reader As New StreamReader(dataStream)

        ' Read the content.
        Dim responseFromServer As String = reader.ReadToEnd()

        ' Display the content.
        Console.WriteLine(responseFromServer)

        ' Clean up the streams and the response.
        reader.Close()
        dataStream.Close()
        response.Close()


    End Sub

End Module

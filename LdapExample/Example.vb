'This example listens for an LDAPv3 BindRequest and replies with a BindResponse
'see http://www.faqs.org/rfcs/rfc2251.html for details

Imports System.Net.Sockets
Imports Kzar.ASN1.BER

Public Module Example
    Public Sub Main()
        Dim listener As New TcpListener(389)
        listener.Start()
        Console.WriteLine("Application started: waiting for an LDAP BindRequest")
        Dim client As TcpClient = listener.AcceptTcpClient()
        Console.WriteLine("Connection accepted")
        client.NoDelay = True
        Dim ns As NetworkStream = client.GetStream()
        Dim rb(client.ReceiveBufferSize) As Byte
        Dim recbytes As Integer = ns.Read(rb, 0, rb.Length)
        Dim be As BERelement = BERelement.DecodePacket(rb)
        Console.Write(be.Serialize())

        Dim messageId As Integer = be.Item(1).GetInteger()
        With be.Item(2)
            Select Case .Type
                Case 96 'APPLICATION 0 -> BindRequest
                    Console.WriteLine("BindRequest")
                    Dim version As Integer = .Item(1).GetInteger()
                    Dim name As String = .Item(2).GetString()
                    Dim authenticationType As Integer = (.Item(3).Type And 31)
                    Dim authentication As String = .Item(3).GetString()

                    Dim br As New BERelement(48)
                    br.AddItem(New BERelement(2, messageId))
                    br.AddItem(New BERelement(97), "BindResponse")
                    With br.Item("BindResponse")
                        .AddItem(New BERelement(10, 50)) 'resultCode = insufficientAccessRights
                        .AddItem(New BERelement(4, "")) 'matchedDN
                        .AddItem(New BERelement(4, "")) 'errorMessage
                    End With
                    Dim sb() As Byte = br.GetEncodedPacket()

                    ns.Write(sb, 0, sb.Length)
                    ns.Flush()
                    Console.WriteLine("Response sent")
            End Select
        End With
        ns.Close()
        client.Close()
        client = Nothing
        listener.Stop()
        listener = Nothing
        Console.WriteLine("Application closed")
    End Sub

End Module


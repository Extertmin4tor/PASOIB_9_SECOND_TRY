'BER(ASN.1) Encode/Decode .Net Library
'Version 1.0
'
'
'Based on ITU-T Rec. X.690 - http://www.itu.int/ITU-T/studygroups/com17/languages/
'Other useful documents:
' - http://luca.ntop.org/Teaching/Appunti/asn1.html
' - http://www.john-wasser.com/ASN1/BasicEncodingRules.html
'
'
'Originally written by Cesare Bianchi
'http://www.cesarebianchi.com
'bcesare@libero.it
'
'
'History:
'Date        Author                   Version  Notes
'2004-12-01  Cesare Bianchi           1.0      First Version


Imports System.Text
Imports System.IO

Public Class BERelement
    Private pItems As Collection
    Private pValue As Byte() = Array.CreateInstance(GetType(Byte), 0)
    Private pType As Byte

#Region "Constructors"
    Public Sub New(ByVal Type As Integer)
        pType = CByte(Type)
        If IsConstructed Then
            pItems = New Collection()
        End If
    End Sub

    Public Sub New(ByVal Type As Integer, ByVal Value As Boolean)
        pType = CByte(Type)
        If IsConstructed Then Throw New NotSupportedException("Unsopported for constructed types")
        ReDim pValue(0)
        pValue(0) = IIf(Value, 1, 0)
    End Sub

    Public Sub New(ByVal Type As Integer, ByVal Value As Integer)
        pType = CByte(Type)
        If IsConstructed Then Throw New NotSupportedException("Unsopported for constructed types")
        Dim i As Integer
        Dim tvalue As Long = Value
        If tvalue < 0 Then tvalue += 1
        If tvalue = 0 Then tvalue = 1
        ReDim pValue(Int(Math.Log(Math.Abs(tvalue)) / Math.Log(256) + 0.125))
        tvalue = Value
        If tvalue < 0 Then tvalue += 256 ^ pValue.Length

        For i = 0 To pValue.Length - 1
            pValue(i) = Int(tvalue / (256 ^ (pValue.Length - i - 1)))
            tvalue = tvalue Mod (256 ^ (pValue.Length - i - 1))
        Next
    End Sub

    Public Sub New(ByVal Type As Integer, ByVal Value As String)
        pType = CByte(Type)
        If IsConstructed Then Throw New NotSupportedException("Unsopported for constructed types")
        If Not (Value Is Nothing) Then pValue = Encoding.UTF8.GetBytes(Value)
    End Sub

    Public Sub New(ByVal Type As Integer, ByVal Content As Byte())
        pType = CByte(Type)
        'primitive type
        If Not IsConstructed Then
            pValue = Content

        Else 'constructed type
            pItems = New Collection()
            Dim tType As Byte
            Dim tContent As Byte()
            Dim cursor, tLength As Integer
            Do While cursor < Content.Length - 1
                tType = Content(cursor)
                cursor += 1
                tLength = GetDecodedLength(Content, cursor) 'this advance cursor too
                ReDim tContent(tLength - 1)
                Array.Copy(Content, cursor, tContent, 0, tLength)
                pItems.Add(New BERelement(tType, tContent))
                cursor += tLength
            Loop

        End If
    End Sub
#End Region

#Region "Properties"
    Public Property Type() As Byte
        Get
            Return pType
        End Get
        Set(ByVal Value As Byte)
            If (Value And 32) = 0 And IsConstructed Then Throw New InvalidCastException("This element is constructed")
            If (Value And 32) > 0 And (Not IsConstructed) Then Throw New InvalidCastException("This element is primitive")
            pType = Value
        End Set
    End Property

    Public ReadOnly Property Items() As BERelement()
        Get
            If Not IsConstructed Then Throw New NotSupportedException("This element is primitive")
            Dim tBer(pItems.Count - 1) As BERelement, i As Integer, tb As BERelement
            For Each tb In pItems
                tBer(i) = tb
                i += 1
            Next
            Return tBer
        End Get
    End Property

    Public ReadOnly Property Item(ByVal Index As Integer) As BERelement
        Get
            If Not IsConstructed Then Throw New NotSupportedException("This element is primitive")
            Return pItems.Item(Index)
        End Get
    End Property

    Public ReadOnly Property Item(ByVal Key As String) As BERelement
        Get
            If Not IsConstructed Then Throw New NotSupportedException("This element is primitive")
            Return pItems.Item(Key)
        End Get
    End Property

    Public ReadOnly Property ByteLength() As Integer
        Get
            If IsConstructed Then
                Dim tl As Integer
                Dim BE As BERelement
                For Each BE In pItems
                    tl += BE.ByteLength + GetEncodedLength(BE.ByteLength).Length + 1
                Next
                Return tl
            Else
                Return pValue.Length
            End If
        End Get
    End Property

    Public ReadOnly Property IsConstructed() As Boolean
        Get
            Return (Type And 32) <> 0
        End Get
    End Property

    Public Property Value() As Byte()
        Get
            If IsConstructed Then Throw New NotSupportedException("This element is constructed")
            Return pValue
        End Get
        Set(ByVal Value As Byte())
            If IsConstructed Then Throw New NotSupportedException("This element is constructed")
            pValue = Value
        End Set
    End Property
#End Region

#Region "Methods"
    Public Function GetString() As String
        If IsConstructed Then Throw New NotSupportedException("This element is constructed")
        Return Encoding.UTF8.GetString(pValue)
    End Function

    Public Function GetInteger() As Integer
        If IsConstructed Then Throw New NotSupportedException("This element is constructed")
        If pValue.Length = 0 Then Return 0
        Dim i As Integer, ret As Long
        For i = 0 To pValue.Length - 1
            ret += (256 ^ (pValue.Length - i - 1)) * pValue(i)
        Next
        If pValue(0) > 127 Then ret -= (256 ^ pValue.Length)
        Return CInt(ret)
    End Function

    Public Function GetBoolean() As Boolean
        If IsConstructed Then Throw New NotSupportedException("This element is constructed")
        If pValue.Length = 0 Then Return False
        Return (pValue(0) <> 0)
    End Function

    Public Sub AddItem(ByVal NewItem As BERelement)
        If Not IsConstructed Then Throw New NotSupportedException("This element is primitive")
        pItems.Add(NewItem)
    End Sub

    Public Sub AddItem(ByVal NewItem As BERelement, ByVal Key As String)
        If Not IsConstructed Then Throw New NotSupportedException("This element is primitive")
        pItems.Add(NewItem, Key)
    End Sub

    Public Function GetEncodedPacket() As Byte()
        Dim ret() As Byte
        Dim tbuf As Byte()
        Dim tstream As New MemoryStream()
        tstream.WriteByte(pType)
        tbuf = GetEncodedLength(ByteLength)
        tstream.Write(tbuf, 0, tbuf.Length)
        If IsConstructed Then
            Dim be As BERelement
            For Each be In pItems
                tbuf = be.GetEncodedPacket()
                tstream.Write(tbuf, 0, tbuf.Length)
            Next
        Else
            tstream.Write(pValue, 0, pValue.Length)
        End If
        Return tstream.ToArray()
    End Function

    Public Function Serialize(Optional ByVal level As Integer = 0) As String
        Try
            Dim ret As String
            ret = Space(level * 5) & "Type: " & pType
            If IsConstructed Then
                ret &= vbCrLf
                Dim be As BERelement
                For Each be In pItems
                    ret &= be.Serialize(level + 1)
                Next

            Else
                Select Case pType
                    Case 1
                        ret &= "   Value: " & GetBoolean() & vbCrLf
                    Case 4, 18 To 27, 128 To 148 'only because in ldap the context-specific non-contructed tag are used only whith strings
                        ret &= "   Value: " & GetString() & vbCrLf
                    Case Else
                        ret &= "   Value: " & GetInteger() & vbCrLf

                End Select
            End If
            Return ret
        Catch e As Exception
            Debug.Write(e.ToString)
        End Try
    End Function
#End Region

#Region "Shared Methods"
    Public Shared Function GetEncodedLength(ByVal Length As Integer) As Byte()
        Dim ret() As Byte
        If Length < 127 Then
            ReDim ret(0)
            ret(0) = Length
        ElseIf Length < 65535 Then
            ReDim ret(2)
            ret(0) = 130
            ret(1) = Int(Length / 256)
            ret(2) = Length Mod 256
        Else
            ReDim ret(4)
            ret(0) = 132
            ret(1) = Int(Length / 256 ^ 3)
            Length = Length - ret(1) * 256 ^ 3
            ret(2) = Int(Length / 256 ^ 2)
            Length = Length - ret(2) * 256 ^ 2
            ret(3) = Int(Length / 256)
            ret(4) = Length Mod 256
        End If
        Return ret
    End Function

    Public Shared Function GetDecodedLength(ByVal Buffer As Byte(), Optional ByRef cursor As Integer = 0) As Integer
        Dim ret As Integer
        If Buffer(cursor) < 128 Then
            ret = Buffer(cursor)
            cursor += 1
            Return ret
        ElseIf Buffer(cursor) = 128 Then
            cursor += 1
            Return -1
        Else
            Dim ll As Integer = Buffer(cursor) - 128, i
            For i = 1 To ll
                ret += Buffer(i + cursor) * (256 ^ (ll - i))
            Next
            cursor += ll + 1
            Return ret
        End If
    End Function

    Public Shared Function DecodePacket(ByVal Buffer As Byte()) As BERelement
        Dim tType As Byte
        Dim tContent As Byte()
        Dim cursor, tLength As Integer
        tType = Buffer(cursor)
        cursor += 1
        tLength = GetDecodedLength(Buffer, cursor) 'this advances cursor too
        ReDim tContent(tLength - 1)
        Array.Copy(Buffer, cursor, tContent, 0, tLength)
        Return New BERelement(tType, tContent)
    End Function
#End Region

End Class

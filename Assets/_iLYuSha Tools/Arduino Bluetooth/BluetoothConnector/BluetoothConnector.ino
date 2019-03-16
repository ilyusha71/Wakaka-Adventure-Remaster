#include <SoftwareSerial.h>
SoftwareSerial BTSerial(10, 11); // RX | TX

char val;
String msg, command;
void setup() 
{
    Serial.begin(9600); 
    BTSerial.begin(9600);
    pinMode(12,OUTPUT);
    digitalWrite(12, HIGH);
Serial.println("WakakaAdventure!");
}

void loop() 
{
    // 若收到「序列埠監控視窗」的資料，則送到藍牙模組
    if (Serial.available()>0) 
    {             
        val = Serial.read();
        if(val != '\n')
            msg += val;
        else
        {
            BTSerial.println(msg); //必須使用println才能與Unity對接，因為設定換行結尾char(10)
            msg = "";
        }
    }
       
    // 若收到藍牙模組的資料，則送到「序列埠監控視窗」
    if (BTSerial.available()) 
    {
        val = BTSerial.read();
        if(val !=  '\n')
            msg += val;
        else
        {
            command = msg;
            Serial.println(msg);
            msg = "";
        }
    }
    
    if(command == "Pass")
    {
        digitalWrite(12, LOW);
        Serial.println("Yes!");
        BTSerial.println("OK! Feedback");
        command = "";
    }
}

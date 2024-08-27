start ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PDL.xml
xcopy /Y GenPackets.cs "../../DummyClient/Packet"
xcopy /Y GenPackets.cs "../../Client/Assets/Scripts/Packet"
xcopy /Y GenPackets.cs "../../Server/Packet"
xcopy /Y ClientPacketManager.cs "../../DummyClient/Packet"
xcopy /Y ClientPacketManager.cs "../../Client/Assets/Scripts/Packet"
xcopy /Y ServerPacketManager.cs "../../Server/Packet"
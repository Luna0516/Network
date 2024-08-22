start ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PDL.xml
xcopy /Y GenPackets.cs "../../DummyClient/Packet"
xcopy /Y GenPackets.cs "../../Server/Packet"
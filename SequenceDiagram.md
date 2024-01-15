# ������ �α���
```mermaid
sequenceDiagram
	actor User
	participant Game Server
	participant Fake Hive Server
	participant DB

	User->> Fake Hive Server: �α��� ��û
	Fake Hive Server -->> User : ������ȣ�� ��ū ����

	User ->> Game Server : ������ȣ�� ��ū�� ���� �α��� ��û
	Game Server -->> Fake Hive Server : ������ȣ�� ��ū�� ��ȿ�� ���� ��û
	Fake Hive Server ->> Game Server : ��ȿ�� ����
	alt ���� ����
	Game Server -->> User : �α��� ���� ����
	end
	
	Game Server ->> DB : ������ȣ�� ���� ���� ������ ��û
	DB -->> Game Server : ���� ������ �ε�
	alt �������� �ʴ� ����
	Game Server -->> User : �α��� ���� ����
	end
	Game Server -->> Game Server : ��ū�� Redis�� ����
	Game Server -->> User : �α��� ���� ����
```

# ���ο� ������ ���� ����

```mermaid
sequenceDiagram
	actor User
	participant Game Server
	participant Fake Hive Server
	participant DB

	User->> Fake Hive Server: �α��� ��û
	Fake Hive Server ->> User : ������ȣ�� ��ū ����

	User ->> Game Server : ������ȣ�� ��ū�� ���� ���� ��û
	Game Server ->> Fake Hive Server : ������ȣ�� ��ū�� ��ȿ�� ���� ��û
	Fake Hive Server -->> Game Server : ��ȿ�� ����
	alt ���� ����
	Game Server -->> User : ���� ���� ���� ����
	end
	
	Game Server ->> DB : ������ȣ�� ���� ������ ��ȸ
	DB -->> Game Server : ������ ��ȸ ���
	alt �̹� ���� ����
	Game Server -->> User : ���� ���� ���� ����
	end

	Game Server ->> DB : �⺻ ������ ����
	alt �⺻ ������ ���� ����
	Game Server ->> DB : RollBack
	Game Server -->> User : ���� ���� ���� ����
	end

	Game Server -->> User : ���� ���� ���� ����

```

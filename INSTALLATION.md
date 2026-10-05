# 🔒 TooliSafe — Quick Guide

An overview of core features, key management, and transport options in **TooliSafe**.

---

## ⚡ Feature Overview

| Feature | Description | Platform / Scope |
| :--- | :--- | :--- |
| **Basic Encryption** | Encrypt files & text to `.tsafe` files[cite: 1] | All platforms[cite: 1] |
| **Diffie-Hellman** | Automated key exchange between contacts[cite: 1] | All platforms[cite: 1] |
| **ShareWith** | Encrypt and transfer via any external messenger[cite: 1] | All platforms[cite: 1] |
| **DirectSend** | 1-click encrypted delivery via Telegram Bot[cite: 1] | Telegram integration[cite: 1] |
| **ZipMode (Win)** | Context menu encryption & auto key management[cite: 2] | Windows[cite: 2] |
| **ZipMode (Android)** | Local encryption without attachment limits[cite: 2] | Android[cite: 2] |
| **Self-Contact ("Me")** | Local backup and encryption[cite: 2] | Built-in[cite: 2] |
| **Import & Export** | AES-256 PIN-protected key backup (`*.dat`)[cite: 2] | All platforms[cite: 2] |

---

## 🔑 Key Features & Workflows

### 1. Basic Functionality
* **Encrypt:** Enter text or choose a file, select a contact + key, and generate a `.tsafe` file[cite: 1].
* **Decrypt:** The recipient opens the `.tsafe` file in TooliSafe using the same key to display original content[cite: 1].

### 2. Automated Key Exchange (Diffie-Hellman)
1. **Initiation:** Sender selects a new contact; TooliSafe transmits a partial key[cite: 1].
2. **Response:** Recipient's app detects the request, returns the second key part, and saves the key locally[cite: 1].
3. **Completion:** Sender receives the remaining key — the shared secret is active[cite: 1].

---

## 📩 Messaging & Transport Options

### ShareWith (Manual Transport)
* **Workflow:** Select text/files $\rightarrow$ Pick recipient $\rightarrow$ Transfer `.tsafe` file via any preferred messenger[cite: 1].
* **Receipt:** Recipient opens `.tsafe` in TooliSafe for automatic decoding[cite: 1].
* **Advantage:** Works flexibly with any messenger service[cite: 1].

### DirectSend (via Telegram)
> [!NOTE]
> **Setup Steps:**
> 1. Message `@TsafeShare_Bot` on Telegram to authorize incoming messages[cite: 1].
> 2. Get your ChatID by messaging `@RawDataBot` (returns `... your ID: xXXXXX`)[cite: 1].
> 3. In TooliSafe, switch to **DirectSend**, enter your ChatID, and set your name[cite: 1].
> 4. Save your contacts' Telegram ChatIDs in TooliSafe contact details[cite: 1].

* **Usage:** Send text and files directly through Telegram with one click[cite: 1].
* **Receipt:** Recipient receives message + `.tsafe` file; content decodes upon opening[cite: 1]. Sender identity is displayed (contact assignment may be required)[cite: 1].

---

## 🛠️ Operating Modes & Key Backup

### 🖥️ ZipMode (Windows)
* **Encrypt:** Right-click any file $\rightarrow$ *Share with TooliSafe*[cite: 2]. Auto-creates a key under *Zip Keys* and a `.tsafe` file in the same directory[cite: 2].
* **Decrypt:** Double-click the `.tsafe` file; TooliSafe opens and decodes automatically[cite: 2]. Save the file to any location[cite: 2].
* **Management:** Review/delete obsolete keys in *Settings $\rightarrow$ Zip Keys*[cite: 2].

### 📱 ZipMode (Android)
* Perform operations directly via the default contact **"Me"**[cite: 2].
* Add attachments or text locally without attachment size/type limits[cite: 2].

### 👤 Self-Contact ("Me")
* Created automatically during installation[cite: 2].
* Encrypt and back up text/files locally without needing ZipMode[cite: 2].
* Created `.tsafe` files can be forwarded through any transport service[cite: 2].

### 💾 Key Import & Export
* Export or restore key pairs and Zip Keys as a `*.dat` file[cite: 2].
* Secured with **AES-256** encryption and protected by your app PIN[cite: 2].

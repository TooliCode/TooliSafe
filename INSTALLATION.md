# 🔒 TooliSafe — Quick Guide

An overview of core features, key management, and transport options in **TooliSafe**.

---

## ⚡ Feature Overview

| Feature | Description | Platform / Scope |
| :--- | :--- | :--- |
| **Basic Encryption** | Encrypt files & text to `.tsafe` files | All platforms |
| **Diffie-Hellman** | Automated key exchange between contacts | All platforms |
| **ShareWith** | Encrypt and transfer via any external messenger | All platforms |
| **DirectSend** | 1-click encrypted delivery via Telegram Bot | Telegram integration |
| **ZipMode (Win)** | Context menu encryption & auto key management | Windows |
| **ZipMode (Android)** | Local encryption without attachment limits | Android |
| **Self-Contact ("Me")** | Local backup and encryption | Built-in |
| **Import & Export** | AES-256 PIN-protected key backup (`*.dat`) | All platforms |

---

## 🔑 Key Features & Workflows

### 1. Basic Functionality
* **Encrypt:** Enter text or choose a file, select a contact + key, and generate a `.tsafe` file.
* **Decrypt:** The recipient opens the `.tsafe` file in TooliSafe using the same key to display original content.

### 2. Automated Key Exchange (Diffie-Hellman)
1. **Initiation:** Sender selects a new contact; TooliSafe transmits a partial key.
2. **Response:** Recipient's app detects the request, returns the second key part, and saves the key locally.
3. **Completion:** Sender receives the remaining key — the shared secret is active.

---

## 📩 Messaging & Transport Options

### ShareWith (Manual Transport)
* **Workflow:** Select text/files → Pick recipient → Transfer `.tsafe` file via any preferred messenger.
* **Receipt:** Recipient opens `.tsafe` in TooliSafe for automatic decoding.
* **Advantage:** Works flexibly with any messenger service.

### DirectSend (via Telegram)
> [!NOTE]
> **Setup Steps:**
> 1. Message `@TsafeShare_Bot` on Telegram to authorize incoming messages.
> 2. Get your ChatID by messaging `@RawDataBot` (returns `... your ID: xXXXXX`).
> 3. In TooliSafe, switch to **DirectSend**, enter your ChatID, and set your name.
> 4. Save your contacts' Telegram ChatIDs in TooliSafe contact details.

* **Usage:** Send text and files directly through Telegram with one click.
* **Receipt:** Recipient receives message + `.tsafe` file; content decodes upon opening. Sender identity is displayed (contact assignment may be required).

---

## 🛠️ Operating Modes & Key Backup

### 🖥️ ZipMode (Windows)
* **Encrypt:** Right-click any file → *Share with TooliSafe*. Auto-creates a key under *Zip Keys* and a `.tsafe` file in the same directory.
* **Decrypt:** Double-click the `.tsafe` file; TooliSafe opens and decodes automatically. Save the file to any location.
* **Management:** Review/delete obsolete keys in *Settings → Zip Keys*.

### 📱 ZipMode (Android)
* Perform operations directly via the default contact **"Me"**.
* Add attachments or text locally without attachment size/type limits.

### 👤 Self-Contact ("Me")
* Created automatically during installation.
* Encrypt and back up text/files locally without needing ZipMode.
* Created `.tsafe` files can be forwarded through any transport service.

### 💾 Key Import & Export
* Export or restore key pairs and Zip Keys as a `*.dat` file.
* Secured with **AES-256** encryption and protected by your app PIN.

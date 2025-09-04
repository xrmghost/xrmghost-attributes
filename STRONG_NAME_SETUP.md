# Strong Name Signing Setup Guide

## Overview

The XrmGhost.Attributes library implements **conditional strong name signing** to eliminate CS8002 warnings for consumers while maintaining seamless local development.

## How It Works

### 🔄 **Conditional Signing Logic**
- **Local Development:** Builds without signing (no setup required)
- **Production CI/CD:** Automatic signing when GitHub secret is configured
- **Graceful Fallback:** No broken builds if signing setup is missing

### 🏗️ **MSBuild Configuration**
The project uses conditional MSBuild properties:
```xml
<SignAssembly Condition="Exists('XrmGhost.Attributes.snk')">true</SignAssembly>
<AssemblyOriginatorKeyFile Condition="Exists('XrmGhost.Attributes.snk')">XrmGhost.Attributes.snk</AssemblyOriginatorKeyFile>
```

## GitHub Repository Setup

### Required Secret

To enable production signing, add this repository secret:

**Secret Name:** `STRONG_NAME_KEY_FULL`  
**Secret Value:** Base64-encoded content of your strong name key file (.snk)

### Steps to Add the Secret

1. **Generate Strong Name Key:**
   ```bash
   sn -k YourProject.snk
   ```

2. **Convert to Base64:**
   ```powershell
   [Convert]::ToBase64String([IO.File]::ReadAllBytes("YourProject.snk"))
   ```

3. **Add to GitHub:**
   - Go to Repository → Settings → Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `STRONG_NAME_KEY_FULL`
   - Value: [paste base64 content]

## Local Development

### ✅ **No Setup Required**
- Clone the repository
- Run `dotnet build` - works immediately
- Assembly built without strong naming
- Full development capabilities maintained

### 🔧 **Optional: Local Strong Name Testing**
If you need to test strong naming locally:

1. Generate a development key:
   ```bash
   sn -k XrmGhost.Attributes.snk
   ```

2. Build with signing:
   ```bash
   dotnet build -c Release
   ```

3. **Important:** The `.snk` file is gitignored and won't be committed

## CI/CD Process

### Automatic Workflow
1. GitHub Actions checks for `STRONG_NAME_KEY_FULL` secret
2. If found, creates `XrmGhost.Attributes.snk` from secret content
3. MSBuild detects key file and enables signing
4. Assembly is built with strong name
5. Verification confirms successful signing
6. Key file is cleaned up for security
7. NuGet package contains signed assembly

### Build Output
- **With Secret:** Strongly-named assembly (`PublicKeyToken` populated)
- **Without Secret:** Warning logged, unsigned assembly built
- **Consumer Impact:** CS8002 warnings eliminated when secret is configured

## Security Considerations

### ✅ **Best Practices Implemented**
- Private key never committed to repository
- Key file automatically cleaned after build
- Conditional logic prevents accidental exposure
- `.gitignore` prevents local key commits

### 🔐 **Key Management**
- Use organization-specific strong name keys for production
- Rotate keys according to security policies
- Monitor secret access in GitHub audit logs
- Consider key escrow for enterprise environments

## Troubleshooting

### Local Build Issues
```bash
# Verify conditional signing works
dotnet build -v normal

# Check if key file exists (should be false locally)
Test-Path "XrmGhost.Attributes.snk"
```

### CI/CD Debugging
- Check GitHub Actions logs for key setup messages
- Verify `STRONG_NAME_KEY_FULL` secret exists
- Ensure Windows build agent has `sn.exe` available
- Review assembly verification step output

### Consumer Testing
```bash
# Verify assembly is signed
sn -vf XrmGhost.Attributes.dll

# Check public key token
sn -T XrmGhost.Attributes.dll
```

## Benefits Summary

| Aspect | Benefit |
|--------|---------|
| **Development** | Zero setup, immediate productivity |
| **Production** | Automatic signing, enterprise ready |
| **Security** | No exposed keys, secure CI/CD |
| **Reliability** | Graceful fallback, no broken builds |
| **Consumer** | Eliminates CS8002 warnings |
| **Maintenance** | Simple workflow, minimal overhead |

---

*This documentation provides setup guidance while maintaining security best practices. No sensitive key material is included in this repository.*

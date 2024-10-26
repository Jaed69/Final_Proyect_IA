# Final_Proyect_IA

### Requisitos previos:
1. **Unity 2023.2 o posterior**: Descarga e instala Unity. Se recomienda usar Unity Hub para gestionar varias versiones.
2. **Python 3.10.12**: Instala esta versión específica de Python. Si estás en Windows, asegúrate de usar la versión x86-64. Se recomienda usar **conda** o **mamba** para gestionar los entornos virtuales de Python.

### Instalación Paso a Paso:

#### 1. Instalar Unity
Descarga Unity desde [aquí](https://unity3d.com/get-unity/download) e instala la versión 2023.2 o superior.

#### 2. Instalar Python 3.10.12
Descarga Python desde [python.org](https://www.python.org/downloads/) e instálalo. Si usas Windows, instala la versión x86-64.

#### 3. Configurar entorno con **conda** (opcional pero recomendado)
Si usas **conda**, ejecuta estos comandos para crear y activar un entorno virtual con Python 3.10.12:

```bash
conda create -n mlagents python=3.10.12
conda activate mlagents
```

#### 4. Instalar el paquete de Unity `com.unity.ml-agents`
1. Abre Unity y navega a `Window > Package Manager`.
2. Haz clic en el botón `+` y selecciona "Add package by name...".
3. Introduce `com.unity.ml-agents` y haz clic en "Add".

#### 5. Instalar los paquetes de Python
1. **Instalar PyTorch en Windows** (opcional, si quieres usar aceleración con CUDA):

   ```bash
   pip3 install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cu121
   ```

2. **Instalar `mlagents` y `mlagents_envs`**:
   
  si prefieres instalar desde PyPi:

   ```bash
   python -m pip install mlagents==1.1.0
   ```

3. **Verificación**: Ejecuta el siguiente comando para verificar la instalación:

   ```bash
   mlagents-learn --help
   ```

FROM pytorch/pytorch:1.11.0-cuda11.3-cudnn8-devel
ENV DEBIAN_FRONTEND noninteractive

# Remove NVIDIA's CUDA repository lists that might cause update issues
RUN rm /etc/apt/sources.list.d/cuda.list
RUN rm /etc/apt/sources.list.d/nvidia-ml.list

# Update and install dependencies
RUN apt-get update && \
    apt-get install -y \
    python3-opencv \
    ca-certificates \
    python3-dev \
    git \
    wget \
    sudo \
    ninja-build \
    ffmpeg

# Symlink python3 to python
RUN ln -sv /usr/bin/python3 /usr/bin/python

# Install pip
RUN wget https://bootstrap.pypa.io/pip/get-pip.py && \
    python3 get-pip.py && \
    rm get-pip.py

# Install Python dependencies
RUN pip3 install cmake  # cmake from apt-get might be too old

# Install additional Python packages as needed
RUN pip3 install 'git+https://github.com/facebookresearch/fvcore'

# Clone and install Detectron2
RUN git clone https://github.com/facebookresearch/detectron2 detectron2_repo
ENV FORCE_CUDA="1"
ARG TORCH_CUDA_ARCH_LIST="Kepler;Kepler+Tesla;Maxwell;Maxwell+Tegra;Pascal;Volta;Turing"
ENV TORCH_CUDA_ARCH_LIST="${TORCH_CUDA_ARCH_LIST}"
RUN pip3 install -e detectron2_repo

# Set a fixed model cache directory.
ENV FVCORE_CACHE="/tmp"

# Set the working directory in the container
WORKDIR /usr/src/app

# Copy the current directory contents into the container at /usr/src/app
COPY . .

# Install any needed packages specified in requirements.txt
RUN pip3 install --no-cache-dir -r requirements.txt

# Make port 80 available to the world outside this container
EXPOSE 80

# Define environment variable
ENV NAME VideoPose3D
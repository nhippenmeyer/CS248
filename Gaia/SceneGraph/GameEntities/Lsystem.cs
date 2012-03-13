﻿﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Gaia.Resources;
using Gaia.Voxels;
using Gaia.Input;
using Gaia.Rendering;
using Gaia.Rendering.RenderViews;

namespace Gaia.SceneGraph.GameEntities
{
    public class Lsystem : Entity
    {
        /*************
         * CONSTANTS *
         *************/
        const int DEFAULT_ITERATIONS = 3;
        const float DEFAULT_FORWARD_LENGTH = 0.6f;
        const float DEFAULT_SPHERE_RADIUS = 0.15f;
        const float DEFAULT_TURN_VALUE = 35.0f;
        const float DEFAULT_VARIATION = 5.0f;

        const char SYMBOL_FORWARD_F = 'F';
        const char SYMBOL_FORWARD_G = 'G';
        const char SYMBOL_FORWARD_NO_DRAW_F = 'f';
        const char SYMBOL_FORWARD_NO_DRAW_G = 'g';
        const char SYMBOL_PITCH_DOWN = '&';
        const char SYMBOL_PITCH_UP = '%';
        const char SYMBOL_POP_MATRIX = ']';
        const char SYMBOL_PUSH_MATRIX = '[';
        const char SYMBOL_ROLL_LEFT = '/';
        const char SYMBOL_ROLL_RIGHT = '\\';
        const char SYMBOL_SPHERE = '@';
        const char SYMBOL_TURN_AROUND = '|';
        const char SYMBOL_TURN_LEFT = '+';
        const char SYMBOL_TURN_RIGHT = '-';

        public struct Point3f
        {
            public float x, y, z;
        };

        public struct ReproductionRule
        {
            public char from;
            public string to;
        };

        string axiom;
        bool dirty;
        int iterations;
        float forwardLength;
        Stack<Matrix> modelViewStack;
        Stack<Matrix> rotationStack, translationStack;
        static Vector3 yDirection = new Vector3(0.0f, 1.0f, 0.0f);
        Vector3 transDirection = yDirection;
        List<ReproductionRule> rules;
        string result;
        float sphereRadius;
        float turnValue;
        float variation;

        List<VoxelGeometry> Voxels;

        byte[] DensityField;
        int DensityFieldSize = 17;  // Keep in powers of 2 + 1
        byte IsoValue = 127;

        public Lsystem()
        {
            this.axiom = "";
            this.iterations = DEFAULT_ITERATIONS;
            this.forwardLength = DEFAULT_FORWARD_LENGTH;
            this.turnValue = DEFAULT_TURN_VALUE;
            this.sphereRadius = DEFAULT_SPHERE_RADIUS;
            this.variation = DEFAULT_VARIATION;
            this.dirty = true;

            modelViewStack = new Stack<Matrix>();
            rotationStack = new Stack<Matrix>();
            translationStack = new Stack<Matrix>();
            rules = new List<ReproductionRule>();
        }

        public Lsystem(string axiom,
                        List<ReproductionRule> rules,
                        int iterations,
                        float forwardLength,
                        float turnValue,
                        float sphereRadius,
                        float variation)
        {
            this.axiom = axiom;
            this.iterations = iterations;
            this.forwardLength = forwardLength;
            this.turnValue = turnValue;
            this.sphereRadius = sphereRadius;
            this.variation = variation;
            this.dirty = true;

            modelViewStack = new Stack<Matrix>();
            rotationStack = new Stack<Matrix>();
            translationStack = new Stack<Matrix>();
            rules = new List<ReproductionRule>();
        }

        public void Destroy()
        {
        }

        /***************************
         * PUBLIC MEMBER FUNCTIONS *
         ***************************/
        public void addRule(ReproductionRule r)
        {
            rules.Add(r);
        }

        public string getResult()
        {
            return result;
        }

        public List<VoxelGeometry> generateGeometry()
        {
            Voxels = new List<VoxelGeometry>();

            if (dirty)
            {
                result = generateResult(axiom, 0);
                Console.Write(result);
                dirty = false;
            }

            // For testing:
            //    result = "G[+&F][-%F[+&F]]GFF@";

            modelViewStack.Push(Transformation.GetTransform());

            initRotationStack();
            initTranslationStack();

            for (int i = 0; i < result.Length; i++)
            {

                float v;
                switch (result[i])
                {
                    case SYMBOL_FORWARD_F:
                        drawLine(forwardLength);
                        translate(forwardLength);
                        break;
                    case SYMBOL_FORWARD_G:
                        drawLine(forwardLength);
                        translate(forwardLength);
                        break;
                    case SYMBOL_FORWARD_NO_DRAW_F:
                        translate(forwardLength);
                        break;
                    case SYMBOL_FORWARD_NO_DRAW_G:
                        translate(forwardLength);
                        break;
                    case SYMBOL_PITCH_DOWN:
                        v = vary(turnValue);
                        rotate(v, 0.0f, 1.0f, 0.0f);
                        break;
                    case SYMBOL_PITCH_UP:
                        v = vary(turnValue);
                        rotate(v, 0.0f, 1.0f, 0.0f);
                        break;
                    case SYMBOL_POP_MATRIX:
                        popMatrix();
                        break;
                    case SYMBOL_PUSH_MATRIX:
                        pushMatrix();
                        break;
                    case SYMBOL_ROLL_LEFT:
                        rotate(turnValue, 1.0f, 0.0f, 0.0f);
                        break;
                    case SYMBOL_ROLL_RIGHT:
                        rotate(-turnValue, 1.0f, 0.0f, 0.0f);
                        break;
                    case SYMBOL_SPHERE:
                        drawSphere(sphereRadius);
                        break;
                    case SYMBOL_TURN_AROUND:
                        rotate(180.0f, 0.0f, 0.0f, 0.0f);
                        break;
                    case SYMBOL_TURN_LEFT:
                        rotate(-turnValue, 0.0f, 0.0f, 1.0f);
                        break;
                    case SYMBOL_TURN_RIGHT:
                        rotate(turnValue, 0.0f, 0.0f, 1.0f);
                        break;
                }

            }

            return Voxels;
        }

        public void setAxiom(string axiom)
        {
            this.axiom = axiom;
            dirty = true;
        }

        public void setForwardLength(float forwardLength)
        {
            this.forwardLength = forwardLength;
        }

        public void setIterations(int iterations)
        {
            this.iterations = iterations;
            dirty = true;
        }

        public void setSphereRadius(float sphereRadius)
        {
            this.sphereRadius = sphereRadius;
        }

        public void setTurnValue(float turnValue)
        {
            this.turnValue = turnValue;
        }

        public void setVariation(float variation)
        {
            this.variation = variation;
        }

        /******************************
         * PROTECTED MEMBER FUNCTIONS *
         ******************************/
        double getNoise()
        {
            Random rand = new Random();
            return rand.NextDouble();
        }

        void drawLine(float length)
        {
            float radius = (DensityFieldSize / 8);
            Vector3 cylinderCenter = Vector3.One * DensityFieldSize * 0.5f;
            DensityField = new byte[DensityFieldSize * DensityFieldSize * DensityFieldSize];

            int initY = DensityFieldSize / 2;

            for (int x = 0; x < DensityFieldSize; x++)
            {
                for (int y = initY; y < DensityFieldSize; y++)
                {
                    for (int z = 0; z < DensityFieldSize; z++)
                    {
                        Vector3 pos = new Vector3(x, y, z);
                        cylinderCenter.Y = pos.Y;
                        float density = Math.Max(1.0f - (pos - cylinderCenter).Length() / radius, 0.0f);
                        DensityField[x + (y + z * DensityFieldSize) * DensityFieldSize] = (byte)(density * 255.0f);
                    }
                }
            }
            VoxelGeometry treeMesh = new VoxelGeometry();
            treeMesh.renderElement.Transform = new Matrix[1] { Matrix.CreateScale(length) * rotationStack.Peek() * translationStack.Peek() };
            treeMesh.GenerateGeometry(ref DensityField, IsoValue, DensityFieldSize, DensityFieldSize, DensityFieldSize, DensityFieldSize - 1, DensityFieldSize - 1, DensityFieldSize - 1, 0, 0, 0, 2.0f / (float)(DensityFieldSize - 1));
            Voxels.Add(treeMesh);


        }

        void drawSphere(float scaleSize)
        {
            float radius = (DensityFieldSize / 4);
            Vector3 cylinderCenter = Vector3.One * DensityFieldSize * 0.5f;
            DensityField = new byte[DensityFieldSize * DensityFieldSize * DensityFieldSize];

            for (int x = 0; x < DensityFieldSize; x++)
            {
                for (int y = 0; y < DensityFieldSize; y++)
                {
                    for (int z = 0; z < DensityFieldSize; z++)
                    {
                        Vector3 pos = new Vector3(x, y, z);
                        float density = Math.Max(1.0f - (pos - cylinderCenter).Length() / radius, 0.0f);
                        DensityField[x + (y + z * DensityFieldSize) * DensityFieldSize] = (byte)(density * 255.0f);
                    }
                }
            }
            VoxelGeometry treeMesh = new VoxelGeometry();
            treeMesh.renderElement.Transform = new Matrix[1] { rotationStack.Peek() * translationStack.Peek() };
            treeMesh.GenerateGeometry(ref DensityField, IsoValue, DensityFieldSize, DensityFieldSize, DensityFieldSize, DensityFieldSize - 1, DensityFieldSize - 1, DensityFieldSize - 1, 0, 0, 0, 2.0f / (float)(DensityFieldSize - 1));
            Voxels.Add(treeMesh);

        }

        /****************************
         * PRIVATE MEMBER FUNCTIONS *
         ****************************/
        void eraseStack()
        {
            while (rotationStack.Count != 0)
            {
                Matrix top = rotationStack.Pop();
                // erase top
            }
            while (translationStack.Count != 0)
            {
                Matrix top = translationStack.Pop();
                // erase top
            }
        }

        string generateResult(string str, int count)
        {
            for (int i = 0; i < str.Length; i++)
            {
                for (int j = 0; j < rules.Count; j++)
                {
                    if (str[i] == rules[j].from)
                    {
                        if (i == str.Length - 1)
                        {
                            str = str.Substring(0, i) + rules[j].to;
                        }
                        else
                        {
                            str = str.Substring(0, i) + rules[j].to + str.Substring(i + 1, str.Length - i - 1);
                        }
                        i += rules[j].to.Length;
                        break;
                    }
                }
            }

            if (count < iterations) return generateResult(str, count + 1);
            else return str;
        }

        void initRotationStack()
        {
            rotationStack.Clear();
            Matrix worldMatrix = Transformation.GetTransform();

            rotationStack.Push(worldMatrix);

            transDirection = Vector3.Transform(yDirection, worldMatrix);
            transDirection.Normalize();
        }

        void initTranslationStack()
        {
            translationStack.Clear();
            translationStack.Push(Matrix.Identity);
        }

        void popMatrix()
        {
            if (rotationStack.Count > 0)
            {
                Matrix top = rotationStack.Pop();
                // delete top

            }

            if (rotationStack.Count == 0)
            {
                initRotationStack();
            }
            else
            {
                transDirection = Vector3.Transform(yDirection, rotationStack.Peek());
                transDirection.Normalize();
            }

            if (translationStack.Count > 0)
            {
                Matrix top = translationStack.Pop();
                // delete top
            }

            if (translationStack.Count == 0)
            {
                initTranslationStack();
            }
        }

        void pushMatrix()
        {
            rotationStack.Push(rotationStack.Peek());
            translationStack.Push(translationStack.Peek());
        }

        void rotate(float r, float rx, float ry, float rz)
        {
            Vector3 axis;
            axis.X = rx;
            axis.Y = ry;
            axis.Z = rz;

            float radians = MathHelper.ToRadians(r);
            Matrix rotatedTop = rotationStack.Pop() * Matrix.CreateFromAxisAngle(axis, radians);
            rotationStack.Push(rotatedTop);

            transDirection = Vector3.Transform(yDirection, rotatedTop);
            transDirection.Normalize();
        }

        void translate(float distance)
        {
            Vector3 axis;
            axis.X = transDirection.X * distance;
            axis.Y = transDirection.Y * distance;
            axis.Z = transDirection.Z * distance;
            Matrix translatedTop = translationStack.Pop() * Matrix.CreateTranslation(axis);
            translationStack.Push(translatedTop);
        }

        float vary(float v)
        {
            float n = (float)getNoise();
            n -= 0.5f;
            float var = (variation * n);
            return v + var;
        }

    }
}
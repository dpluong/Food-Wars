using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowHighlight : MonoBehaviour
{
    
    Dictionary<Renderer, Material[]> glowMaterialDict = new Dictionary<Renderer, Material[]>();
    Dictionary<Renderer, Material[]> originalMaterialDict = new Dictionary<Renderer, Material[]>();

    Dictionary<Color, Material> cachedGlowMaterialDict = new Dictionary<Color, Material>();

    public Material glowMaterial;
    
    private bool isGlowing = false;

    private Color validSpaceColor = Color.green;
    private Color originalGlowColor;

    private void Awake() 
    {
        PrepareMaterialDicts();
        originalGlowColor = glowMaterial.GetColor("_GlowColor");
    }

    private void PrepareMaterialDicts()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            Material[] originalMaterials = renderer.materials;
            originalMaterialDict.Add(renderer, originalMaterials);
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < originalMaterials.Length; ++i)
            {
                Material mat = null;
                if (cachedGlowMaterialDict.TryGetValue(originalMaterials[i].color, out mat) == false)
                {
                    mat = new Material(glowMaterial);
                    mat.color = originalMaterials[i].color;
                    cachedGlowMaterialDict[mat.color] = mat;
                }
                newMaterials[i] = mat;
            }
            glowMaterialDict.Add(renderer, newMaterials);
        }
    }

    internal void ResetGlowHighlight()
    {
        foreach (Renderer renderer in glowMaterialDict.Keys)
        {
            foreach (Material item in glowMaterialDict[renderer])
            {
                item.SetColor("_GlowColor", originalGlowColor);
            }           
        }
    }

    internal void HighlightValidPath()
    {
        if (isGlowing == false)
            return;
        foreach (Renderer renderer in glowMaterialDict.Keys)
        {
            foreach (Material item in glowMaterialDict[renderer])
            {
                item.SetColor("_GlowColor", validSpaceColor);
            }
        }
    }

    public void ToggleGlow()
    {
        if (isGlowing == false)
        {
            ResetGlowHighlight();
            foreach (Renderer renderer in originalMaterialDict.Keys)
            {
                renderer.materials = glowMaterialDict[renderer];
            }
        }
        else
        {
            foreach (Renderer renderer in originalMaterialDict.Keys)
            {
                renderer.materials = originalMaterialDict[renderer];
            }
        }
        isGlowing = !isGlowing;
    }

    public void ToggleGlow(bool state)
    {
        if (isGlowing == state)
            return;
        isGlowing = !state;
        ToggleGlow();
    }
}

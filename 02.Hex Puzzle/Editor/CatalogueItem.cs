
using UnityEngine;
public abstract class CatalogueItem
{
	private bool isSelected;
	public abstract string CatalogueName { get;}
	public bool IsSelected
	{
		get => isSelected;
		set => isSelected = value;
	}
	
	public abstract void Init();
	public abstract void OnSelected();
	public abstract void OnDeSelected();
	
	public abstract void DrawUI(Rect position);
	
	protected abstract void DrawButtons(Rect position);
}

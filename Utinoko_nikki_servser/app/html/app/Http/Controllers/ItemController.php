<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class ItemController extends Controller
{
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');

        if ($id !== null && $id !== '') {
            $item = DB::table('items')
                ->select('id', 'name', 'type')
                ->where('id', $id)
                ->first();

            return response()->json($item);
        }

        $items = DB::table('items')
            ->select('id', 'name', 'type')
            ->get();

        return response()->json($items);
    }
}
